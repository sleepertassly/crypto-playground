using AutoMapper;
using CryptoPlayground.Data;
using CryptoPlayground.Data.Migrations;
using CryptoPlayground.Models;
using CryptoPlayground.Models.CipherViewModels;
using CryptoPlayground.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPlayground.Controllers
{
	[Authorize(Roles = ApplicationRole.Puzzler)]
	public class CipherController : Controller
	{
		private readonly IMapper _mapper;
		private readonly ILogger _logger;
		private readonly ITeamManager _teamManager;
		private readonly ApplicationDbContext _context;
		private readonly IStringLocalizer<CipherController> _localizer;

		const int DefaultRemainingAttempts = 5;

		public CipherController(
			IMapper mapper,
			ILogger<CipherController> logger,
			ITeamManager teamManager,
			ApplicationDbContext context,
			IStringLocalizer<CipherController> localizer)
		{
			_mapper = mapper;
			_logger = logger;
			_context = context;
			_teamManager = teamManager;
			_localizer = localizer;
		}

		public async Task<IActionResult> Index(string cipher, string error)
		{
			ViewBag.Error = error;
			ViewBag.Cipher = cipher;
			ViewBag.LocalizedCipher = _localizer[cipher];
			ViewBag.KeyPlaceholder = cipher == "Affine" ? _localizer["key1,key2"] : _localizer["key"];
			ViewBag.JsonKeyPlaceholder = String.Format("[{{Id:{0}_1,Key:\"{1}\"}}, {{Id:{0}_2,Key:\"{1}\"}}, ... ,{{Id:{0}_n,Key:\"{1}\"}}]", _localizer["id"], ViewBag.KeyPlaceholder);
			var team = _teamManager.GetTeamBy(User);
			var letters = await _context.Letters.Include("UnlockedBy").Where(l => l.Cipher == cipher).ToListAsync();
			var model = new List<CipherViewModel>();
			foreach (var letter in letters)
			{
				TeamLetter teamLetter = letter.UnlockedBy.SingleOrDefault(lt => lt.TeamId == team.Id);
				if (teamLetter == null)
				{
					model.Add(new CipherViewModel()
					{
						Id = letter.Id,
						Key = letter.Key,
						Status = LetterStatus.Locked,
						RemainingAttempts = DefaultRemainingAttempts
					});
				}
				else
				{
					model.Add(new CipherViewModel()
					{
						Id = letter.Id,
						Key = letter.Key,
						UnlockedOn = teamLetter.UnlockedOn,
						RemainingAttempts = teamLetter.RemainingAttempts,
						Status = teamLetter.UnlockedOn == null || teamLetter.RemainingAttempts <= 0 ? LetterStatus.Locked : LetterStatus.Unlocked
					});
				}
			}
			return View(model);
		}

		public async Task<IActionResult> Download(int id)
		{
			var letter = await _context.Letters.Include("UnlockedBy").SingleOrDefaultAsync(t => t.Id == id);
			if (letter == null)
			{
				return NotFound();
			}

			byte[] fileBytes = null;
			var team = _teamManager.GetTeamBy(User);
			if (letter.UnlockedBy.Select(l => l.TeamId).Contains(team.Id))
			{
				fileBytes = Encoding.ASCII.GetBytes(letter.Content);
			}
			else
			{
				fileBytes = Encoding.ASCII.GetBytes(letter.EncryptedContent);
			}

			return File(fileBytes, "application/force-download", String.Format("{0}.txt", id));
		}

		public IActionResult DownloadAll(string cipher)
		{
			return File(String.Format("~/archive/{0}.zip", cipher), "application/force-download", String.Format("{0}.zip", cipher));
		}

		public async Task<IActionResult> Upload(int id, string key)
		{
			var letter = await _context.Letters.Include("UnlockedBy").SingleOrDefaultAsync(t => t.Id == id);
			if (letter == null)
			{
				return NotFound();
			}

			var team = _teamManager.GetTeamBy(User);
			var teamLetter = letter.UnlockedBy.SingleOrDefault(tl => tl.TeamId == team.Id);
			if (teamLetter == null)
			{
				if (KeyEquals(letter.Key,key))
				{
					letter.UnlockedBy.Add(new TeamLetter()
					{
						TeamId = team.Id,
						RemainingAttempts = DefaultRemainingAttempts - 1,
						UnlockedOn = DateTime.Now.ToUniversalTime()
					});
				}
				else
				{
					letter.UnlockedBy.Add(new TeamLetter()
					{
						TeamId = team.Id,
						RemainingAttempts = DefaultRemainingAttempts - 1
					});
				}
			}
			else if (teamLetter.RemainingAttempts > 0 && teamLetter.UnlockedOn == null)
			{
				if (KeyEquals(letter.Key, key))
				{
					teamLetter.UnlockedOn = DateTime.Now.ToUniversalTime();
				}
				else
				{
					teamLetter.RemainingAttempts--;
				}
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index), new { cipher = letter.Cipher });
		}

		public async Task<IActionResult> UploadAll(string cipher, string keys)
		{
			IEnumerable<LetterKey> letterKeys;
			try
			{
				letterKeys = JsonConvert.DeserializeObject<List<LetterKey>>(keys)
					.OrderBy(lk => lk.Id);
			}
			catch (JsonException)
			{
				return RedirectToAction(nameof(Index), new { cipher = cipher, error = "json-format" });
			}
			var team = _teamManager.GetTeamBy(User);
			var letterIds = letterKeys.Select(lk => lk.Id).ToList();
			var letters = _context.Letters.Include("UnlockedBy").Where(l => letterIds.Contains(l.Id)).ToDictionary(k => k.Id, v => v);
			foreach (var lk in letterKeys)
			{
				var teamLetter = letters[lk.Id].UnlockedBy.SingleOrDefault(tl => tl.TeamId == team.Id);
				if (teamLetter == null)
				{
					if (KeyEquals(letters[lk.Id].Key, lk.Key))
					{
						letters[lk.Id].UnlockedBy.Add(new TeamLetter()
						{
							TeamId = team.Id,
							RemainingAttempts = DefaultRemainingAttempts - 1,
							UnlockedOn = DateTime.Now.ToUniversalTime()
						});
					}
					else
					{
						letters[lk.Id].UnlockedBy.Add(new TeamLetter()
						{
							TeamId = team.Id,
							RemainingAttempts = DefaultRemainingAttempts - 1
						});
					}
				}
				else if (teamLetter.RemainingAttempts > 0 && teamLetter.UnlockedOn == null)
				{
					if (KeyEquals(letters[lk.Id].Key, lk.Key))
					{
						teamLetter.UnlockedOn = DateTime.Now.ToUniversalTime();
					}
					else
					{
						teamLetter.RemainingAttempts--;
					}
				}
			}
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index), new { cipher = cipher });
		}


        private bool KeyEquals(string key1, string key2)
        {
            return !String.IsNullOrEmpty(key1) && !String.IsNullOrEmpty(key2) && key1.ToLower().Trim() == key2.ToLower().Trim();
        }
    }
}
using AutoMapper;
using CryptoPlayground.Data;
using CryptoPlayground.Models;
using CryptoPlayground.Models.CipherViewModels;
using CryptoPlayground.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index(string cipher)
        {
            ViewBag.KeyPlaceholder = cipher == "Affine" ? _localizer["key1,key2"]: _localizer["key"];
            ViewBag.Cipher = _localizer[cipher];
			var team = _teamManager.GetTeamBy(User);
			var letters = await _context.Letters.Include("UnlockedBy").Where(l => l.Cipher == cipher).ToListAsync();
			var model = new List<CipherViewModel>();
			foreach (var letter in letters)
			{
				TeamLetter teamLetter = letter.UnlockedBy.SingleOrDefault(lt => lt.TeamId == team.Id);
				if (teamLetter != null)
				{
					model.Add(new CipherViewModel()
					{
						Id = letter.Id,
						Key = letter.Key,
						UnlockedOn = teamLetter.UnlockedOn,
						Status = LetterStatus.Unlocked
					});
				}
				else
				{
					model.Add(new CipherViewModel()
					{
						Id = letter.Id,
						Key = letter.Key,
						Status = LetterStatus.Locked
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

			if (letter.Key == key)
			{
				var team = _teamManager.GetTeamBy(User);
				if (!letter.UnlockedBy.Select(l => l.TeamId).Contains(team.Id))
				{
					letter.UnlockedBy.Add(new TeamLetter()
					{
						TeamId = team.Id,
						UnlockedOn = DateTime.Now.ToUniversalTime()
					});
					await _context.SaveChangesAsync();
				}
			}

			return RedirectToAction(nameof(Index), new { cipher = letter.Cipher });
        }

		public async Task<IActionResult> UploadAll(string cipher, string keys)
		{
			var letterKeys = JsonConvert.DeserializeObject<List<LetterKey>>(keys).OrderBy(lk => lk.Id);
			var team = _teamManager.GetTeamBy(User);
			var letterIds = letterKeys.Select(lk => lk.Id).ToList();
			var letters = _context.Letters.Include("UnlockedBy").Where(l => letterIds.Contains(l.Id)).ToDictionary(k => k.Id, v => v);
			foreach (var lk in letterKeys)
			{
				if (letters[lk.Id].Key == lk.Key)
				{
					if (!letters[lk.Id].UnlockedBy.Select(l => l.TeamId).Contains(team.Id))
					{
						letters[lk.Id].UnlockedBy.Add(new TeamLetter()
						{
							TeamId = team.Id,
							UnlockedOn = DateTime.Now.ToUniversalTime()
						});
						await _context.SaveChangesAsync();
					}
				}
			}
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index), new { cipher = cipher });
		}
    }
}
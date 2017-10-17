using AutoMapper;
using AutoMapper.QueryableExtensions;
using CryptoPlayground.Data;
using CryptoPlayground.Models;
using CryptoPlayground.Models.CipherViewModels;
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
    public class CipherController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<CipherController> _localizer;

        public CipherController(
            IMapper mapper,
            ILogger<CipherController> logger,
            ApplicationDbContext context,
            IStringLocalizer<CipherController> localizer)
        {
            _mapper = mapper;
            _logger = logger;
            _context = context;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index(string cipher)
        {
            ViewBag.KeyPlaceholder = "key";
            ViewBag.Cipher = _localizer[cipher];
            return View(await _context.Letters.Where(l => l.Cipher == cipher).ProjectTo<CipherViewModel>().ToListAsync());
        }

        public async Task<IActionResult> Download(int id)
        {
            var letter = await _context.Letters.SingleOrDefaultAsync(t => t.Id == id);
            if (letter == null)
            {
                return NotFound();
            }

            byte[] fileBytes;
            if (letter.Status == Models.LetterStatus.Locked)
            {
                fileBytes = Encoding.ASCII.GetBytes(letter.EncryptedContent);
            }
            else
            {
                fileBytes = Encoding.ASCII.GetBytes(letter.Content);
            }

            return File(fileBytes, "application/force-download", String.Format("{0}.txt", id));
        }

        public IActionResult DownloadAll(string cipher)
        {
            return File(String.Format("~/archive/{0}.zip", cipher), "application/force-download", String.Format("{0}.zip", cipher));
        }

        public async Task<IActionResult> Upload(int id, string key)
        {
            var letter = await _context.Letters.SingleOrDefaultAsync(t => t.Id == id);
            if (letter == null)
            {
                return NotFound();
            }

            if (letter.Status == Models.LetterStatus.Locked)
            {
                var message = String.Empty;
                if (letter.Key == key)
                {
                    letter.Status = Models.LetterStatus.Unlocked;
                    letter.UnlockedOn = DateTime.Now.ToUniversalTime();
                    message = _localizer["Success"];
                }
                else
                {
                    letter.Attempts++;
                    message = _localizer["Error!"];
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { cipher = letter.Cipher, message = message });
            }

            return RedirectToAction(nameof(Index), new { cipher = letter.Cipher });
        }

        public async Task<IActionResult> UploadAll(string cipher, string keys)
        {
            var letterKeys = JsonConvert.DeserializeObject<List<LetterKey>>(keys).OrderBy(lk => lk.Id);

            var letterIds = letterKeys.Select(lk => lk.Id).ToList();
            var letters = _context.Letters.Where(l => letterIds.Contains(l.Id)).ToDictionary(k => k.Id, v => v);
            foreach (var lk in letterKeys)
            {
                if (letters[lk.Id].Status == LetterStatus.Locked)
                {
                    if (letters[lk.Id].Key == lk.Key)
                    {
                        letters[lk.Id].Status = LetterStatus.Unlocked;
                        letters[lk.Id].UnlockedOn = DateTime.Now.ToUniversalTime();
                    }
                    else
                    {
                        letters[lk.Id].Attempts++;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { cipher = cipher });
        }
    }
}
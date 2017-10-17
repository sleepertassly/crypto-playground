using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CryptoPlayground.Data;
using Microsoft.AspNetCore.Identity;
using CryptoPlayground.Models;

namespace CryptoPlayground.Controllers
{
    public class AppController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Reset()
        {
            var messages = new List<string>();

            _context.Database.EnsureCreated();
            messages.Add("The database is created and it is up to date.");
            
            await CreateUserAsync("Administrator", "admin@mailtrap.com", "P@ssw0rd", ApplicationRole.Administrator);
            messages.Add(String.Format("Role '{0}' is created.", ApplicationRole.Administrator));
            messages.Add(String.Format("User '{0}' is created.", "Administrator"));

            return View(messages);
        }
      
        public IActionResult CipherReset()
        {
            var messages = new List<string>();

            _context.Database.EnsureCreated();
            messages.Add("The database is created and it is up to date.");

            // Removes all the lettes
            _context.Letters.RemoveRange(_context.Letters.ToList());

            // Colects the letters from the files
            var letters = GetLetters(@".\Letters\Letters of Pliny by the Younger Pliny").OrderBy(l => l.Length);

            // Caesar cipher encrypted letters
            var caesarCipher = new CaesarCipher();
            GenerateLetters(caesarCipher, letters.Take((int)(letters.Count() * 0.1)));
            messages.Add(String.Format("{0} encrypted letters were created.", caesarCipher.Name));
            Archive(caesarCipher.Name);
            messages.Add(String.Format("{0} archive created.", caesarCipher.Name));

            return View("Reset", messages);
        }

        /// <summary>
        /// Creats the specified role if it doesn't exist.
        /// </summary>
        /// <param name="roleName">The role-name to be verified and created if it doesn't exist.</param>
        private async Task<IdentityRole> CreateRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                await _roleManager.CreateAsync(role);
            }
            return role;
        }

        private async Task<ApplicationUser> CreateUserAsync(string userName, string email, string password, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var role = await CreateRoleAsync(roleName);
                user = new ApplicationUser { UserName = userName, Email = email, EmailConfirmed = true };
                await _userManager.CreateAsync(user, password);
                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(userName), roleName);
            }
            return user;
        }

        private IEnumerable<string> GetLetters(string folderPath)
        {
            var result = new List<string>();
            foreach (string file in System.IO.Directory.EnumerateFiles(folderPath, "*.txt"))
            {
                result.Add(System.IO.File.ReadAllText(file));
            }
            return result;
        }

        private void Archive(string cipher)
        {
            const string temp = @".\Temp";
            const string archive = @".\wwwroot\archive";
            var archiveFileName = String.Format("{0}.zip", cipher);

            // Removes the old archive if exits
            System.IO.File.Delete(System.IO.Path.Combine(archive, archiveFileName));
            // Empty the temp folder
            System.IO.Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));

            var letters = _context.Letters.Where(l => l.Cipher == cipher).ToList();
            foreach (var letter in letters)
            {
                // Copies the selected files to the temp folder
                System.IO.File.WriteAllText(System.IO.Path.Combine(temp, String.Format("{0}.txt", letter.Id)), letter.EncryptedContent);
            }

            // Creates a new archive
            System.IO.Compression.ZipFile.CreateFromDirectory(temp, System.IO.Path.Combine(archive, archiveFileName));
        }

        private void GenerateLetters(ICipher cipher, IEnumerable<string> letters)
        {
            foreach (var letter in letters)
            {
                var key = cipher.RandomKey();
                _context.Letters.Add(new Letter()
                {
                    Cipher = cipher.Name,
                    Key = key,
                    Content = letter,
                    EncryptedContent = cipher.Encrypt(key, letter),
                    Status = LetterStatus.Locked,
                });
            }
            _context.SaveChanges();
        }
    }
}
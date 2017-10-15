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
    }
}
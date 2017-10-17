using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CryptoPlayground.Models;
using CryptoPlayground.Data;
using Microsoft.AspNetCore.Identity;

namespace CryptoPlayground.Services
{
	public class TeamManager : ITeamManager
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public TeamManager(
			ApplicationDbContext context,
		   UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public Team GetTeamBy(ClaimsPrincipal principal)
		{
			var userId =_userManager.GetUserId(principal);
			return userId != null ? _context.Teams.FirstOrDefault(t => t.TeamMembers.Select(tm => tm.Id).Contains(userId)) : null;
		}
	}
}

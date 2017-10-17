using CryptoPlayground.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CryptoPlayground.Services
{
    public interface ITeamManager
    {
		Team GetTeamBy(ClaimsPrincipal principal);
	}
}

using AutoMapper;
using CryptoPlayground.Models;
using CryptoPlayground.Models.UserViewModels;
using System.Linq;

namespace CryptoPlayground
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Team, TeamViewModel>();
            CreateMap<TeamViewModel, Team>();
        }
    }
}

using AutoMapper;
using CryptoPlayground.Models;
using CryptoPlayground.Models.CipherViewModels;
using CryptoPlayground.Models.TeamViewModels;
using System.Linq;

namespace CryptoPlayground
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Team, TeamViewModel>()
                .ForMember(vm => vm.TeamMembers, opt => opt.MapFrom(e => e.TeamMembers.Select(x => x.Id)));
            CreateMap<TeamViewModel, Team>()
                .ForMember(vm => vm.TeamMembers, opt => opt.Ignore());
            CreateMap<Letter, CipherViewModel>();
            CreateMap<CipherViewModel, Letter>();
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CryptoPlayground.Models.TeamViewModels
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Team members")]
        public IEnumerable<string> TeamMembers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CryptoPlayground.Models.TeamViewModels
{
    public class TeamIndexViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Score")]
        public int Score { get; set; }
        [Display(Name = "Last Unlocked On")]
        public DateTime? LastUnlockedOn { get; set; }
    }
}

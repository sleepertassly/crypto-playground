using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CryptoPlayground.Models.TeamViewModels
{
    public class TeamIndexViewModel : TeamViewModel
    {
        [Display(Name = "Score")]
        public int Score { get; set; }
        [Display(Name = "Last Unlocked On")]
        public DateTime? LastUnlockedOn { get; set; }
    }
}

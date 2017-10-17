using System;
using System.ComponentModel.DataAnnotations;

namespace CryptoPlayground.Models.CipherViewModels
{
    public class CipherViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Key")]
        public string Key { get; set; }
        [Display(Name = "Unlocked On")]
        public DateTime? UnlockedOn { get; set; }
        [Display(Name = "Status")]
        public LetterStatus Status { get; set; }
    }

	public enum LetterStatus
	{
		Locked,
		Unlocked
	}
}

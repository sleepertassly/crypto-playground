using System;
using System.Collections.Generic;

namespace CryptoPlayground.Models
{
    public class Letter
    {
        public int Id { get; set; }
        public string Cipher { get; set; }
        public string Key { get; set; }
        public string Content { get; set; }
        public string EncryptedContent { get; set; }

		public ICollection<TeamLetter> UnlockedBy { get; set; }
	}
}

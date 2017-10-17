using System;

namespace CryptoPlayground.Models
{
    public class Letter
    {
        public int Id { get; set; }
        public string Cipher { get; set; }
        public string Key { get; set; }
        public string Content { get; set; }
        public string EncryptedContent { get; set; }
        public int Attempts { get; set; }
        public DateTime? UnlockedOn { get; set; }
        public LetterStatus Status { get; set; }
    }

    public enum LetterStatus
    {
        Locked,
        Unlocked
    }
}

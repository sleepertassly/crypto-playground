using System;

namespace CryptoPlayground.Models
{
    public abstract class Cipher : ICipher
    {
        protected Random _random = new Random();
        public virtual string Name { get; } = "cipher";
        public virtual string KeyFormat { get; } = "key";

        public abstract string RandomKey();
        public abstract string Decrypt(string key, string text);
        public abstract string Encrypt(string key, string text);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoPlayground.Models
{
    public interface ICipher
    {
        string Name { get; }

        string RandomKey();
        string Decrypt(string key, string text);
        string Encrypt(string key, string text);
    }
}

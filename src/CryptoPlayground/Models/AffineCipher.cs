using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPlayground.Models
{
    public class AffineCipher : Cipher
    {
        public override string Name { get; } = "Affine";

        private IDictionary<char, int> reversAbc = new Dictionary<char, int>()
        {
            { 'a', 0 }, { 'A', 0 },
            { 'b', 1 }, { 'B', 1 },
            { 'c', 2 }, { 'C', 2 },
            { 'd', 3 }, { 'D', 3 },
            { 'e', 4 }, { 'E', 4 },
            { 'f', 5 }, { 'F', 5 },
            { 'g', 6 }, { 'G', 6 },
            { 'h', 7 }, { 'H', 7 },
            { 'i', 8 }, { 'I', 8 },
            { 'j', 9 }, { 'J', 9 },
            { 'k', 10 }, { 'K', 10 },
            { 'l', 11 }, { 'L', 11 },
            { 'm', 12 }, { 'M', 12 },
            { 'n', 13 }, { 'N', 13 },
            { 'o', 14 }, { 'O', 14 },
            { 'p', 15 }, { 'P', 15 },
            { 'q', 16 }, { 'Q', 16 },
            { 'r', 17 }, { 'R', 17 },
            { 's', 18 }, { 'S', 18 },
            { 't', 19 }, { 'T', 19 },
            { 'u', 20 }, { 'U', 20 },
            { 'v', 21 }, { 'V', 21 },
            { 'w', 22 }, { 'W', 22 },
            { 'x', 23 }, { 'X', 23 },
            { 'y', 24 }, { 'Y', 24 },
            { 'z', 25 }, { 'Z', 25 },
        };

        private int[] posibleAs = { 1, 3, 5, 7, 9, 11, 15, 17, 19, 21, 23, 25 };
        private char[] abc = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public override string Decrypt(string key, string text)
        {
            StringBuilder result = new StringBuilder();
            var keys = key.Split(",");
            var a = int.Parse(keys[0]);
            var b = int.Parse(keys[1]);
            foreach (var c in text)
            {
                if (reversAbc.ContainsKey(c))
                {
                    var tmp = (Inverse(a) * (reversAbc[c] - b)) % abc.Length;
                    if (tmp < 0)
                    {
                        result.Append(abc[tmp + abc.Length]);
                    }
                    else
                    {
                        result.Append(abc[tmp]);
                    }
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        public override string Encrypt(string key, string text)
        {
            StringBuilder result = new StringBuilder();
            var keys = key.Split(",");
            var a = int.Parse(keys[0]);
            var b = int.Parse(keys[1]);
            foreach (var c in text)
            {
                if (reversAbc.ContainsKey(c))
                {
                    result.Append(abc[(reversAbc[c] * a + b) % abc.Length]);
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        public override string RandomKey()
        {
            return String.Format("{0},{1}", posibleAs[_random.Next(posibleAs.Length)], _random.Next(1, 26));
        }

        private int Inverse(int a)
        {
            for (int i = 1; i < abc.Length; i++)
            {
                if ((a * i) % abc.Length == 1)
                {
                    return i;
                }
            }

            throw new Exception("Number has no invers!");
        }

    }
}

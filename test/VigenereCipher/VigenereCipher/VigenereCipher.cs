using System;
using System.Collections.Generic;
using System.Text;

namespace VigenereCipher
{
    class VigenereCipher
    {
        private Random _random = new Random();

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

        private char[] abc = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        private string[] keys = {
            "about",
            "all",
            "also",
            "and",
            "as",
            "at",
            "be",
            "because",
            "but",
            "by",
            "can",
            "come",
            "could",
            "day",
            "do",
            "even",
            "find",
            "first",
            "for",
            "from",
            "get",
            "give",
            "go",
            "have",
            "he",
            "her",
            "here",
            "him",
            "his",
            "how",
            "I",
            "if",
            "in",
            "into",
            "it",
            "its",
            "just",
            "know",
            "like",
            "look",
            "make",
            "man",
            "many",
            "me",
            "more",
            "my",
            "new",
            "no",
            "not",
            "now",
            "of",
            "on",
            "one",
            "only",
            "or",
            "other",
            "our",
            "out",
            "people",
            "say",
            "see",
            "she",
            "so",
            "some",
            "take",
            "tell",
            "than",
            "that",
            "the",
            "their",
            "them",
            "then",
            "there",
            "these",
            "they",
            "thing",
            "think",
            "this",
            "those",
            "time",
            "to",
            "two",
            "up",
            "use",
            "very",
            "want",
            "way",
            "we",
            "well",
            "what",
            "when",
            "which",
            "who",
            "will",
            "with",
            "would",
            "year",
            "you",
            "your"
        };

        public string Decrypt(string key, string text)
        {
            StringBuilder result = new StringBuilder();
            var index = 0;
            foreach (var c in text)
            {
                var offset = reversAbc[key[(index++) % key.Length]];
                if (reversAbc.ContainsKey(c))
                {
                    var tmp = (reversAbc[c] - offset) % abc.Length;
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

        public string Encrypt(string key, string text)
        {
            StringBuilder result = new StringBuilder();
            var index = 0;
            foreach (var c in text)
            {
                var offset = reversAbc[key[(index++) % key.Length]];
                if (reversAbc.ContainsKey(c))
                {
                    result.Append(abc[(reversAbc[c] + offset) % abc.Length]);
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        public string RandomKey()
        {
            return keys[_random.Next(keys.Length)];
        }
    }
}

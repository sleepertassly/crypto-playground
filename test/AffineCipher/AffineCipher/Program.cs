using System;

namespace AffineCipher
{
    class Program
    {
        static void Main(string[] args)
        {
            var abc = "abcdefghijklmnopqrstuvwxyz";

            var cipher = new AffineCipher();
            for (int i = 0; i < 2000; i++)
            {
                var key = cipher.RandomKey();
                var encryptedText = cipher.Encrypt(key, abc);
                var decryptdText = cipher.Decrypt(key, encryptedText);
                if (abc != decryptdText)
                {
                    throw new Exception();
                }
            }

            Console.WriteLine("test succeeded");
            Console.ReadKey();
        }
    }
}

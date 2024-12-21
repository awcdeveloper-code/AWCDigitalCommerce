using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AWC.DigitalCommerce.TicketsController
{
    public class AesEncryption
    {
        public static string EncryptText(string text, string password)
        {
            // Generate a random salt
            byte[] salt = GenerateRandomBytes(16);

            // Derive a key from the password and salt
            using (var keyDerivationFunction = new Rfc2898DeriveBytes(password, salt, 100000))
            {
                byte[] key = keyDerivationFunction.GetBytes(32); // AES-256 key length
                byte[] iv = GenerateRandomBytes(16); // AES block size

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                            {
                                streamWriter.Write(text);
                            }

                            byte[] encryptedData = memoryStream.ToArray();
                            return Convert.ToBase64String(Combine(salt, iv, encryptedData));
                        }
                    }
                }
            }
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            byte[] bytes = new byte[length];

            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(bytes);
            }
            return bytes;
        }

        private static byte[] Combine(params byte[][] arrays)
        {
            byte[] result = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;

            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }
            return result;
        }
    }
}

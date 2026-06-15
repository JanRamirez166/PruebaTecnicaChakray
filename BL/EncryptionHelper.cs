using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class EncryptionHelper
    {

        private const string KeyEnvVar = "AbC123xYz456MnO789pQr012StUv345s";
        private const string IVEnvVar = "abcd1234efgh5678";

        private static byte[] Key => GetKeyOrIV(KeyEnvVar, 32);
        private static byte[] IV => GetKeyOrIV(IVEnvVar, 16);

        private static byte[] GetKeyOrIV(string value, int requiredLength)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != requiredLength)
            {
                throw new InvalidOperationException(
                    $"Environment variable '{value}' must be set and exactly {requiredLength} characters long.");
            }
            return Encoding.UTF8.GetBytes(value);
        }

        /// Encripta en el algoritmo AES-256.
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        /// Desencripta en el algoritmo AES-256.
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}

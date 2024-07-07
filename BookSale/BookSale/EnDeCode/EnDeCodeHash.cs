using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace BookSale.EnDeCode
{
    public class EncryptionHelper
    {
        private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("0123456789ABCDEF"); // 16 byte (128-bit)
        private static readonly byte[] IV = new byte[16];
        
        private static byte[] GenerateEncryptionKey()
        {
            // Anahtarı rastgele oluştur
            byte[] key = new byte[32]; // 256 bit (32 byte) uzunlukta bir anahtar kullanalım
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }

            return key;
        }

        private static byte[] GenerateIV()
        {
            // IV'yi rastgele oluştur
            byte[] iv = new byte[16]; // 128 bit (16 byte) uzunlukta bir IV kullanalım
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }

            return iv;
        }

        public static string EncryptId(int id)
        {
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = EncryptionKey;
                rijAlg.IV = IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                byte[] idBytes = Encoding.UTF8.GetBytes(id.ToString());

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(idBytes, 0, idBytes.Length);
                        csEncrypt.FlushFinalBlock();
                        return WebEncoders.Base64UrlEncode(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static int DecryptId(string encryptedId)
        {
            byte[] encryptedBytes = WebEncoders.Base64UrlDecode(encryptedId);

            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = EncryptionKey;
                rijAlg.IV = IV;

                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            string decryptedString = srDecrypt.ReadToEnd();
                            if (int.TryParse(decryptedString, out int decryptedId))
                            {
                                return decryptedId;
                            }
                            else
                            {
                                throw new FormatException("Şifre çözme işlemi başarısız oldu.");
                            }
                        }
                    }
                }
            }
        }
    }
}
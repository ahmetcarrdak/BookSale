/*
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace BookSale.EnDeCode
{
    public class EncryptionHelperPass
    {
        private static readonly byte[] EncryptionKey = GenerateEncryptionKey();
        private static readonly byte[] IV = GenerateIV();

        private static byte[] GenerateEncryptionKey()
        {
            byte[] key = new byte[32]; // 256-bit key
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }
            return key;
        }

        private static byte[] GenerateIV()
        {
            byte[] iv = new byte[16]; // 128-bit IV
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }
            return iv;
        }

        public static string EncryptId(string pass)
        {
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = EncryptionKey;
                rijAlg.IV = IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                byte[] idBytes = Encoding.UTF8.GetBytes(pass);

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

        public static string DecryptId(string encryptedId)
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
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}

*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace BookSale.EnDeCode
{
    public class EncryptionHelperPass
    {
        private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("0123456789ABCDEF"); // 16 byte (128-bit)
        private static readonly byte[] IV = new byte[16];


        public static string EncryptId(string pass)
        {
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = EncryptionKey;
                rijAlg.IV = IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                byte[] idBytes = Encoding.UTF8.GetBytes(pass);

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

        public static string DecryptId(string encryptedId)
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
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}

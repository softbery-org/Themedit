// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SfbLibrary.Security
{
    /// <summary>
    /// Cryptography
    /// </summary>
    public static class Cryptography
    {
        /// <summary>
        /// Salt
        /// </summary>
        public static string Salt { get; set; } = "jfi32jkxs7654csadf";

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>String</returns>
        public static string Encrypt(this string text)
        {
            try
            {
                var result = "";
                byte[] data = UTF8Encoding.UTF8.GetBytes(text);

                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Salt));
                    using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                    {
                        ICryptoTransform transform = tripDes.CreateEncryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        result = Convert.ToBase64String(results, 0, results.Length);
                    }
                }
                return result;
            }catch(Exception ex)
            {
                Console.WriteLine($"[{ex.HResult}]: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="text">Decrypted text</param>
        /// <returns>String</returns>
        public static string Decrypt(this string text)
        {
            try
            {
                var result = "";
                byte[] data = Convert.FromBase64String(text);

                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Salt));
                    using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                    {
                        ICryptoTransform transform = tripDes.CreateDecryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        result = UTF8Encoding.UTF8.GetString(results);
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{ex.HResult}]: {ex.Message}");
                return null;
            }
        }
    }
}

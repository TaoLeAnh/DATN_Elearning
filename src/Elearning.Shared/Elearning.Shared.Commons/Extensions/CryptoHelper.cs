using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public static class CryptoHelper
    {
        /// <summary>
        /// Tính MD5 hash của input và trả về chuỗi hex (32 ký tự).
        /// </summary>
        public static string Md5Hash(string input)
        {
            using var md5 = MD5.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(bytes);
            var sb = new StringBuilder(32);
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// Tính SHA-256 hash của input và trả về chuỗi hex (64 ký tự).
        /// </summary>
        public static string Sha256Hash(string input)
        {
            using var sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder(64);
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public static class AESCrypto
    {
        private static readonly string defaultKeyAESCrypto = "5iLUuL2R5kgoDn07c9UcJEOhF7cpQCWf";

        // Mã hóa dữ liệu với IV ngẫu nhiên
        public static string Encrypt(string? plainText, string key = "", bool safeForUrl = false)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText), "Đầu vào không hợp lệ");

            if (string.IsNullOrEmpty(key))
                key = defaultKeyAESCrypto;

            if (key.Length != 32)
                throw new ArgumentOutOfRangeException(nameof(key), "Key đầu vào không hợp lệ. Độ dài phải là 32 byte");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.GenerateIV(); // Tạo IV ngẫu nhiên

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Ghi IV vào đầu stream
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    byte[] result = msEncrypt.ToArray();
                    return safeForUrl ? ToBase64Url(result) : Convert.ToBase64String(result);
                }
            }
        }

        // Giải mã dữ liệu với IV từ cipher text
        public static string Decrypt(string? cipherText, string key = "", bool safeForUrl = false)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText), "Đầu vào không hợp lệ");

            if (string.IsNullOrEmpty(key))
                key = defaultKeyAESCrypto;

            if (key.Length != 32)
                throw new ArgumentOutOfRangeException(nameof(key), "Key đầu vào không hợp lệ. Độ dài phải là 32 byte");

            byte[] fullCipher = safeForUrl ? FromBase64Url(cipherText) : Convert.FromBase64String(cipherText);

            if (fullCipher.Length < 16)
                throw new ArgumentException("Dữ liệu mã hóa không hợp lệ");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);

                // Tách IV từ 16 byte đầu
                byte[] iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, 16);
                aesAlg.IV = iv;

                // Lấy phần cipher text thực sự (bỏ 16 byte IV đầu)
                byte[] cipherBytes = new byte[fullCipher.Length - 16];
                Array.Copy(fullCipher, 16, cipherBytes, 0, cipherBytes.Length);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        // Mã hóa và trả về Hex với IV ngẫu nhiên
        public static string EncryptToHex(string plainText, string key = "")
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText), "Đầu vào không hợp lệ");

            if (string.IsNullOrEmpty(key))
                key = defaultKeyAESCrypto;

            if (key.Length != 32)
                throw new ArgumentOutOfRangeException(nameof(key), "Key đầu vào không hợp lệ. Độ dài phải là 32 byte");

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV(); // Tạo IV ngẫu nhiên

                using (MemoryStream ms = new MemoryStream())
                {
                    // Ghi IV vào đầu
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(plainBytes, 0, plainBytes.Length);
                    }

                    return BitConverter.ToString(ms.ToArray()).Replace("-", "");
                }
            }
        }

        // Giải mã từ Hex với IV
        public static string DecryptFromHex(string hexText, string key = "")
        {
            if (string.IsNullOrEmpty(hexText))
                throw new ArgumentNullException(nameof(hexText), "Đầu vào không hợp lệ");

            if (string.IsNullOrEmpty(key))
                key = defaultKeyAESCrypto;

            if (key.Length != 32)
                throw new ArgumentOutOfRangeException(nameof(key), "Key đầu vào không hợp lệ. Độ dài phải là 32 byte");

            // Chuyển Hex thành byte array
            byte[] fullCipher = new byte[hexText.Length / 2];
            for (int i = 0; i < fullCipher.Length; i++)
            {
                fullCipher[i] = Convert.ToByte(hexText.Substring(i * 2, 2), 16);
            }

            if (fullCipher.Length < 16)
                throw new ArgumentException("Dữ liệu mã hóa không hợp lệ");

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);

                // Tách IV từ 16 byte đầu
                byte[] iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, 16);
                aes.IV = iv;

                // Lấy phần cipher text thực sự
                byte[] cipherBytes = new byte[fullCipher.Length - 16];
                Array.Copy(fullCipher, 16, cipherBytes, 0, cipherBytes.Length);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        // Phương thức để tạo key ngẫu nhiên an toàn
        public static string GenerateKey()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] keyBytes = new byte[32];
                rng.GetBytes(keyBytes);
                return Convert.ToBase64String(keyBytes);
            }
        }

        private static string ToBase64Url(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return Convert.FromBase64String(padded);
        }
    }
}

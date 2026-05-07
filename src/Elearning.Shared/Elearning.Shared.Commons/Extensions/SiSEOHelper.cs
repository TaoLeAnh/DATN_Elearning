using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Elearning.Shared.Commons.Extensions
{
    public static class SiSEOHelper
    {
        #region Tạo seo link
        public static string GenerateSeoLink(string title)
        {
            string slug = Slugify(title);

            long unixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string shortId = ToBase36(unixSeconds);

            int maxLength = 400 - shortId.Length - 1;
            if (slug.Length > maxLength)
                slug = slug.Substring(0, maxLength);

            return $"{slug}-{shortId}";
        }
        private static string Slugify(string text)
        {
            text = text.ToLowerInvariant();
            text = RemoveDiacritics(text);
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", "-").Trim('-');
            return text;
        }
        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        private static string ToBase36(long value)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            var sb = new StringBuilder();
            while (value > 0)
            {
                sb.Insert(0, chars[(int)(value % 36)]);
                value /= 36;
            }
            return sb.ToString();
        }
        #endregion
    }
}

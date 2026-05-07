using Elearning.Shared.Commons.Model.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Elearning.Shared.Commons.Extensions
{
    public static class SIConvert
    {
        public static string NumberToString(string number)
        {
            string[] dv = { "", "mươi", "trăm", "nghìn", "triệu", "tỉ" };
            string[] cs = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string doc;
            int i, j, k, n, len, found, ddv, rd;

            len = number.Length;
            number += "ss";
            doc = "";
            found = 0;
            ddv = 0;
            rd = 0;

            i = 0;
            while (i < len)
            {
                //So chu so o hang dang duyet
                n = (len - i + 2) % 3 + 1;

                //Kiem tra so 0
                found = 0;
                for (j = 0; j < n; j++)
                {
                    if (number[i + j] != '0')
                    {
                        found = 1;
                        break;
                    }
                }

                //Duyet n chu so
                if (found == 1)
                {
                    rd = 1;
                    for (j = 0; j < n; j++)
                    {
                        ddv = 1;
                        switch (number[i + j])
                        {
                            case '0':
                                if (n - j == 3) doc += cs[0] + " ";
                                if (n - j == 2)
                                {
                                    if (number[i + j + 1] != '0') doc += "lẻ ";
                                    ddv = 0;
                                }
                                break;
                            case '1':
                                if (n - j == 3) doc += cs[1] + " ";
                                if (n - j == 2)
                                {
                                    doc += "mười ";
                                    ddv = 0;
                                }
                                if (n - j == 1)
                                {
                                    if (i + j == 0) k = 0;
                                    else k = i + j - 1;

                                    if (number[k] != '1' && number[k] != '0')
                                        doc += "mốt ";
                                    else
                                        doc += cs[1] + " ";
                                }
                                break;
                            case '5':
                                if (i + j == len - 1)
                                    doc += "lăm ";
                                else
                                    doc += cs[5] + " ";
                                break;
                            default:
                                doc += cs[number[i + j] - 48] + " ";
                                break;
                        }

                        //Doc don vi nho
                        if (ddv == 1)
                        {
                            doc += dv[n - j - 1] + " ";
                        }
                    }
                }


                //Doc don vi lon
                if (len - i - n > 0)
                {
                    if ((len - i - n) % 9 == 0)
                    {
                        if (rd == 1)
                            for (k = 0; k < (len - i - n) / 9; k++)
                                doc += "tỉ ";
                        rd = 0;
                    }
                    else
                        if (found != 0) doc += dv[(len - i - n + 1) % 9 / 3 + 2] + " ";
                }

                i += n;
            }

            if (len == 1)
                if (number[0] == '0' || number[0] == '5') return cs[number[0] - 48];

            return doc;
        }
        public static int ToInt(object value)
        {
            if (value == null) return 0;
            int valueConvert = 0;
            int.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static int ToInt(string value)
        {
            if (value == null) return 0;
            int valueConvert = 0;
            int.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static uint ToUint(object value)
        {
            if (value == null) return 0;
            uint valueConvert = 0;
            uint.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static uint ToUint(string value)
        {
            if (value == null) return 0;
            uint valueConvert = 0;
            uint.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static double ToDouble(object value)
        {
            if (value == null) return 0;
            double valueConvert = 0;
            double.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static double ToDouble(string value)
        {
            if (value == null) return 0;
            double valueConvert = 0;
            double.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static decimal ToDecimal(object value)
        {
            if (value == null) return 0;
            decimal valueConvert = 0;
            decimal.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static decimal ToDecimal(string value)
        {
            if (value == null) return 0;
            decimal valueConvert = 0;
            decimal.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static long ToLong(object value)
        {

            if (value == null) return 0;
            long valueConvert = 0;
            long.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static long ToLong(string value)
        {

            if (value == null) return 0;
            long valueConvert = 0;
            long.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static ulong ToUlong(object value)
        {

            if (value == null) return 0;
            ulong valueConvert = 0;
            ulong.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static ulong ToUlong(string value)
        {

            if (value == null) return 0;
            ulong valueConvert = 0;
            ulong.TryParse(value.ToString(), out valueConvert);
            return valueConvert;
        }
        public static bool ToBoolean(object value)
        {
            if (value == null)
                return false;

            string svalue = value?.ToString()?.ToUpper() ?? string.Empty;

            if (svalue == "ON" || svalue == "1" || svalue == "TRUE")
                return true;

            return false;
        }

        public static bool ToBoolean(string value)
        {

            if (value == null) return false;
            string svalue = value.ToString().ToUpper();
            if (svalue == "ON" || svalue == "1" || svalue == "TRUE")
                return true;
            return false;
        }


        public static DateTime? EnsureUtc(this DateTime? dateTime)
        {
            return dateTime?.EnsureUtc();
        }
        public static DateTime EnsureUtc(this DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Utc => dateTime,
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
                _ => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            };
        }
        public static DateTime? ToDateTimeFormat(string value, string shortDatePattern = "dd/MM/yyyy")
        {
            if (string.IsNullOrEmpty(value)) return null;
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = shortDatePattern;
            return Convert.ToDateTime(value, dtfi);
        }
        public static string DateTimeToString(DateTime dateTime, string format = "dd/MM/yyyy")
        {
            if (dateTime == DateTime.MinValue)
            {
                return string.Empty;
            }
            return dateTime.ToString(format);
        }

        /// <summary>
        /// 2021-06-12T11:23:31Z
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shortDatePattern"></param>
        /// <returns></returns>
        public static DateTime? ConvertToDateTimeHHmmzz(string value, string shortDatePattern = "yyyy-MM-ddTHH:mm:ssZ")
        {
            if (string.IsNullOrEmpty(value)) return null;
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = shortDatePattern;
            return Convert.ToDateTime(value, dtfi);
        }
        public static DateTime? ToDateTime(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            DateTime dtOut;
            if (DateTime.TryParse(value, out dtOut))
                return dtOut;
            return null;
        }
        public static DateTime? ToDateTime(object value)
        {
            if (value == null) return null;
            DateTime dtOut;
            if (DateTime.TryParse(value.ToString(), out dtOut))
                return dtOut;
            return null;


        }
        public static string ToSha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }
        public static string SanitizeFileName(string pathObject)
        {
            string originalPath = pathObject;
            string[] pathParts = originalPath.Split('/');
            string fileName = pathParts[pathParts.Length - 1];



            // Loại bỏ các ký tự không hợp lệ trong tên file
            fileName = Regex.Replace(fileName, @"[^\w\-. ]", "");

            // Thay thế khoảng trắng bằng dấu gạch dưới
            fileName = fileName.Replace(" ", "_");

            // Loại bỏ các dấu chấm liên tiếp
            fileName = Regex.Replace(fileName, @"\.{2,}", ".");

            // Đảm bảo tên file không bắt đầu hoặc kết thúc bằng dấu chấm
            fileName = fileName.Trim('.');

            // Giới hạn độ dài của tên file (ví dụ: 255 ký tự)
            if (fileName.Length > 255)
            {
                fileName = fileName.Substring(0, 255);
            }


            string sanitizedFileName = fileName;
            string newPath = string.Join("/", pathParts.Take(pathParts.Length - 1).Append(sanitizedFileName));

            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }
            return newPath;

        }


        public static List<Guid> GetIDsFromString(string ids)
        {
            List<Guid> lstValues = new List<Guid>();
            if (!string.IsNullOrEmpty(ids))
            {
                string[] temp = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                lstValues = temp
                    .Select(x => x.Trim())
                    .Where(x => Guid.TryParse(x, out _))
                    .Select(Guid.Parse)
                    .ToList();
            }
            return lstValues;
        }

        // <summary>
        /// Lấy DescriptionAttribute của 1 giá trị enum, nếu không có thì trả về chính tên enum.
        /// </summary>
        public static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                return attribute == null ? value.ToString() : attribute.Description;
            }
            return string.Empty;
        }
        public static T? FindEnumByTitle<T>(string title) where T : struct, Enum
        {
            // Kiểm tra theo tên Enum (không phân biệt hoa thường)
            if (Enum.TryParse<T>(title, true, out var result))
            {
                return result;
            }
            return null;
        }
        /// <summary>
        /// Chuyển bất kỳ enum nào thành danh sách EnumItem { Id, Code, Value }.
        /// </summary>
        public static List<EnumItem> ToEnumList<TEnum>() where TEnum : Enum
        {
            var type = typeof(TEnum);

            // Lấy toàn bộ giá trị của enum
            return Enum.GetValues(type)
                .Cast<TEnum>()
                .Select(e =>
                {
                    // Id = giá trị số
                    int id = Convert.ToInt32(e);

                    // Code = tên enum
                    string code = e.ToString();

                    // Value = DescriptionAttribute nếu có, ngược lại lấy code
                    string value = GetEnumDescription(e);

                    return new EnumItem
                    {
                        Id = id,
                        Code = code,
                        Value = value
                    };
                })
                .ToList();
        }


        public static string CreateSlug(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Convert to lowercase
            input = RemoveVietnameseTone(input);
            string slug = input.ToLowerInvariant();
            var normalized = slug.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            slug = sb.ToString().Normalize(NormalizationForm.FormC);

            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-");

            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Remove multiple hyphens
            slug = Regex.Replace(slug, @"\-{2,}", "-").Trim('-');

            return slug;
        }

        public static string EscapeElasticQuery(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string[] specialChars = { "/", ":" };

            foreach (var ch in specialChars)
            {
                input = input.Replace(ch, "\\" + ch);
            }
            return input;
        }
        public static string GetPropertyDesk<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda)
        {
            if (propertyLambda.Body is MemberExpression member)
            {
                return member.Member.Name;
            }
            throw new ArgumentException("Biểu thức không hợp lệ!");
        }



    }
}

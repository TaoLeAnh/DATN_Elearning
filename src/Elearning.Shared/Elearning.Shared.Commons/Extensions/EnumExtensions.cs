using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Lấy giá trị Description của Enum. Nếu không có trả về ToString().
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            if (value == null) return string.Empty;
            FieldInfo? field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }
    }
}

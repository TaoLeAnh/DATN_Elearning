using Elearning.Shared.Commons.Model.Extentions.Blazors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public static class FluentSelectExtensions
    {
        /// <summary>
        /// Đánh dấu Selected = true cho item có Value khớp với giá trị enum,
        /// và Selected = false cho các item còn lại.
        /// </summary>
        public static void SelectByEnum<TEnum>(this List<FluentSelectItem> list, TEnum enumValue)
            where TEnum : Enum
        {
            // Chuyển enumValue thành string phù hợp với FluentSelectItem.Value
            // (nếu bạn lưu Id dưới dạng string thì dùng Convert.ToInt32; 
            // nếu lưu Code thì dùng enumValue.ToString()).
            string target = Convert.ToInt32(enumValue).ToString();

            foreach (var item in list)
            {
                item.Selected = item.Value == target;
            }
        }

    }

}

using Elearning.Shared.Commons.Enums;
using Elearning.Shared.Commons.Model.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public class SIPermissionTreeBuilder
    {
        /// <summary>
        /// Build ra tree node
        /// </summary>
        /// <returns></returns>
        public static List<PermissionNode> BuildTree()
        {
            // 1) Lấy toàn bộ enum (trừ None)
            var all = Enum.GetValues<EnumPermissions>()
                .Cast<EnumPermissions>()
                .Where(e => e != EnumPermissions.None)
                .Select(e => new PermissionNode
                {
                    Code = e.ToString(),
                    Value = (int)(object)e,
                    Name = GetDescription(e) ?? e.ToString(),
                    ParentCode = GetParentCode(e) // suy ra cha
                })
                .ToDictionary(x => x.Code, x => x);

            // 2) Gắn con vào cha
            foreach (var node in all.Values)
            {
                if (node.ParentCode != null && all.TryGetValue(node.ParentCode, out var parent))
                {
                    parent.Children.Add(node);
                }
            }

            // 3) Trả về các root (ParentCode == null)
            return all.Values.Where(x => x.ParentCode == null).ToList();
        }

        private static string? GetDescription(EnumPermissions e)
            => e.GetType().GetField(e.ToString())?
                 .GetCustomAttributes(typeof(DescriptionAttribute), false)
                 .Cast<DescriptionAttribute>().FirstOrDefault()?.Description;

        private static string? GetParentCode(EnumPermissions e)
        {
            var name = e.ToString();
            var idx = name.LastIndexOf('_');
            if (idx > 0)
            {
                var parentName = name[..idx];
                if (Enum.TryParse<EnumPermissions>(parentName, out _)) return parentName;
            }
            // fallback theo số
            var val = (int)e;
            if (val % 100 != 0)
            {
                var baseVal = (val / 100) * 100;
                if (Enum.IsDefined(typeof(EnumPermissions), baseVal))
                    return ((EnumPermissions)baseVal).ToString();
            }
            return null;
        }
    }
}

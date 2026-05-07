using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Commons.Enums
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PermissionDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public PermissionDescriptionAttribute(string description)
        {
            Description = description;
        }
    }




    public enum EnumPermissions
    {
        [Description("Chưa có quyền")]
        None = 0,

        [Description("Siêu quản trị")]
        [PermissionDescription("Đầy đủ toàn bộ quyền của hệ thống")]
        SIEUQUANTRI = 50,


    }
}

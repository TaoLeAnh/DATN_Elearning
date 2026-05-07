using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Commons.Enums
{
    /// <summary>
    /// Danh sách các quyền (Roles) dùng chung giữa các hệ thống.
    /// </summary>
    public enum EnumRoles
    {
        /// <summary>
        /// Không xác định
        /// </summary>
        [Description("Chưa xác định")]
        None = 0,

        /// <summary>
        /// Quản trị hệ thống (SuperUser)
        /// </summary>
        [Description("Quản trị hệ thống")]
        QuanTriHeThong = 20,

    }
}

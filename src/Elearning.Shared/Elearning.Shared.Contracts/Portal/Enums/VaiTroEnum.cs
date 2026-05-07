using System.ComponentModel;

namespace Elearning.Shared.Contracts.Portal.Enums
{
    public enum EnumVaiTro
    {
        [Description("Quản trị viên")]
        Admin = 1,

        [Description("Giảng viên")]
        GiangVien = 2,

        [Description("Học sinh")]
        HocSinh = 3
    }
}

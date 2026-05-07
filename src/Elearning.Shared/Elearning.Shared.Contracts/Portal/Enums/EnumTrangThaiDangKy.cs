using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Enums
{
    public enum EnumTrangThaiDangKy
    {
        [Description("Chờ duyệt")]
        ChoDuyet = 0,

        [Description("Đang học")]
        DangHoc = 1,

        [Description("Hoàn thành")]
        HoanThanh = 2,

        [Description("Đã hủy")]
        DaHuy = 3,

        [Description("Bị khóa")]
        BiKhoa = 4
    }
}

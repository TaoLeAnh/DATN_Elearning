using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Enums
{
    public enum EnumLoaiViPham
    {
        [Description("Chuyển tab trình duyệt")]
        ChuyenTab = 1,

        [Description("Thoát chế độ toàn màn hình")]
        ThoatToanManHinh = 2,

        [Description("Mất kết nối Internet (Offline)")]
        MatKetNoi = 3,

        [Description("Cố tình copy/paste nội dung")]
        CopyPaste = 4,

        [Description("Lý do khác")]
        Khac = 99
    }
}

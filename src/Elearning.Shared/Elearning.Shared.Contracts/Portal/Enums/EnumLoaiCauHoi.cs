using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Enums
{
    public enum EnumLoaiCauHoi
    {
        [Description("Một lựa chọn (Radio)")]
        MotLuaChon = 1,

        [Description("Nhiều lựa chọn (Checkbox)")]
        NhieuLuaChon = 2,

        [Description("Tự luận / Điền khuyết")]
        TuLuan = 3
    }
}

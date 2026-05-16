using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Enums
{
    public enum EnumLoaiCauHoi
    {
        [Description("Trắc nghiệm (Một lựa chọn)")]
        MotLuaChon = 1,

        [Description("Mệnh đề (Đúng/Sai)")]
        MenhDeDungSai = 2,

        [Description("Điền kết quả (Đáp án ngắn)")]
        DienKetQua = 3
    }
}

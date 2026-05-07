using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Enums
{
    public enum EnumTrangThaiBaiLam
    {
        [Description("Đang làm bài")]
        DangLam = 1,

        [Description("Đã nộp bài")]
        DaNop = 2,

        [Description("Đã chấm điểm")]
        DaCham = 3,

        [Description("Vi phạm/Hủy kết quả")]
        Huy = 4
    }
}

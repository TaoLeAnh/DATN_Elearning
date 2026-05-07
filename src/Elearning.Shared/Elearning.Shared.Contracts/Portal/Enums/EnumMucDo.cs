using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Enums
{
    public enum EnumMucDo
    {
        [Description("Dễ")]
        De = 1,

        [Description("Trung bình")]
        TrungBinh = 2,

        [Description("Khó")]
        Kho = 3,

        [Description("Rất khó")]
        RatKho = 4
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Enums
{
    public enum EnumModuleType
    {
        //[Description("-- Chọn loại module --")]
        //None = 0,
        [Description("Module hệ thống")]
        HeThong = 1,
        [Description("Module nghiệp vụ chính")]
        NghiepVu = 2,
        [Description("Module bổ sung thêm")]
        ModuleNgoai = 3,
        [Description("Module khác")]
        ModuleKhac = 4,
        [Description("Module nghiệp vụ khác")]
        NghiepVuKhac = 5,
        [Description("Chưa được phân loại")]
        KhongXacDinh = 99
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Enums
{
    public enum OrganizationUnitType : byte
    {
        [Description("Đơn vị")]
        DonVi = 1,      // “Đơn vị”
        [Description("Phòng ban")]
        PhongBan = 2    // “Phòng ban”
    }
}

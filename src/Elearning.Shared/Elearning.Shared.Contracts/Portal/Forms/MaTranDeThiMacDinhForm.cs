using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class MaTranDeThiMacDinhForm
    {
        public MonHocEnum MonHoc { get; set; }
        public string TenMaTran { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<ChiTietMaTranMacDinhForm> ChiTiets { get; set; } = new();
    }
}

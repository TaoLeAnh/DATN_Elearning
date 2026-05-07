using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms.KyThi
{
    public class CauHinhDeThiForm
    {
        public List<CauHoiKyThiItemForm> DanhSachCauHoi { get; set; } = new();
    }

    public class CauHoiKyThiItemForm
    {
        public Guid CauHoiId { get; set; }
        public EnumLoaiPhanThi PhanThi { get; set; }
        public int ThuTu { get; set; }
    }
}

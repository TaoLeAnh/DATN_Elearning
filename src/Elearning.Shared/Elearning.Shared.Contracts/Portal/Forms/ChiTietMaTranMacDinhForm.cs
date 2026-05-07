using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class ChiTietMaTranMacDinhForm
    {
        public EnumLoaiPhanThi PhanThi { get; set; }
        public EnumLoaiCauHoi LoaiCauHoi { get; set; }
        public EnumMucDo MucDo { get; set; }
        public string ChuDe { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }
}

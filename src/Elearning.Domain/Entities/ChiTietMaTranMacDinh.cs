using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class ChiTietMaTranMacDinh : BaseDomainEntity
    {
        public Guid MaTranDeThiMacDinhId { get; set; }

        public EnumLoaiPhanThi PhanThi { get; set; }
        public EnumLoaiCauHoi LoaiCauHoi { get; set; }
        public EnumMucDo MucDo { get; set; }

        public string ChuDe { get; set; } = string.Empty; // Tên chuyên đề/chương
        public int SoLuong { get; set; } // Số lượng câu cần bốc

        // Navigation
        public virtual MaTranDeThiMacDinh MaTran { get; set; } = default!;
    }
}

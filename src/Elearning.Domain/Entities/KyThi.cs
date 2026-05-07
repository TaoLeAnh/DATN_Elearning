using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class KyThi : BaseDomainEntity
    {
        public string TenKyThi { get; set; } = default!;

        public Guid? KhoaHocId { get; set; }

        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public int ThoiLuongPhut { get; set; }

        public MonHocEnum? MonHoc { get; set; }

        public EnumLoaiDeThi? LoaiDeThi { get; set; }

        public int? NamThi { get; set; } // Ví dụ: 2025, 2026

        public string? TinhThanh { get; set; } // Ví dụ: "Hà Nội", "Bắc Ninh"

        public string? TenTruong { get; set; } // Ví dụ: "THPT Chuyên KHTN"

        public bool IsPublic { get; set; } = true;

        public virtual KhoaHoc? KhoaHoc { get; set; }

        public virtual ICollection<BaiLam> BaiLams { get; set; } = new List<BaiLam>();
        public virtual ICollection<CauHoiKyThi> CauHoiKyThis { get; set; } = new List<CauHoiKyThi>();
    }
}

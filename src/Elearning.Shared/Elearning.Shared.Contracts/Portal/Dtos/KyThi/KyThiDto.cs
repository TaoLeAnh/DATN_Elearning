using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos.KyThi
{
    public class KyThiDto : BaseEntiyDto
    {
        public int STT { get; set; }

        public string TenKyThi { get; set; } = default!;

        public Guid KhoaHocId { get; set; }
        public MonHocEnum? MonHoc { get; set; }
        public EnumLoaiDeThi? LoaiDeThi { get; set; }
        public int? NamThi { get; set; }
        public string? TinhThanh { get; set; }
        public string? TenTruong { get; set; }
        public bool IsPublic { get; set; }
        public string? TenKhoaHoc { get; set; }

        public DateTime? ThoiGianBatDau { get; set; }

        public DateTime? ThoiGianKetThuc { get; set; }

        public int ThoiLuongPhut { get; set; }

        public int SoLuongCauHoi { get; set; }
        public int SoLuongBaiLam { get; set; }
    }
}

using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class PublicKyThiDto
    {
        public Guid Id { get; set; }
        public string TenKyThi { get; set; } = string.Empty;
        public int ThoiLuongPhut { get; set; }
        public int SoLuongCauHoi { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public MonHocEnum? MonHoc { get; set; }
        public EnumLoaiDeThi? LoaiDeThi { get; set; }
        public int? NamThi { get; set; }
        public string? TinhThanh { get; set; }
    }
}

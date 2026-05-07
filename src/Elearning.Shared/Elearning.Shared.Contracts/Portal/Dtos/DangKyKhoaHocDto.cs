using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class DangKyKhoaHocDto : BaseEntiyDto
    {
        public int STT { get; set; }

        public Guid NguoiDungId { get; set; }
        public string? TenNguoiDung { get; set; }
        public string? EmailNguoiDung { get; set; }

        public Guid KhoaHocId { get; set; }
        public string? TenKhoaHoc { get; set; }

        public DateTime NgayDangKy { get; set; }

        public EnumTrangThaiDangKy TrangThai { get; set; }

        public double TienDo { get; set; }
    }
}

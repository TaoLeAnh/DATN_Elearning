using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class HoSoGiaoVienDto: BaseEntiyDto
    {
        public int STT { get; set; }

        public Guid NguoiDungId { get; set; }

        public string? TenGiaoVien { get; set; } // Lấy từ bảng NguoiDung sang cho tiện hiển thị

        public string? AnhDaiDienUrl { get; set; }

        public MonHocEnum MonHocChuyenMon { get; set; }

        public string? ThanhTichNoiBat { get; set; }

        public string? PhuongPhapGiangDay { get; set; }
    }
}

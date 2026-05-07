using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class HoSoGiaoVien : BaseDomainEntity
    {
        // Khóa ngoại trỏ về NguoiDung
        public Guid NguoiDungId { get; set; }

        public string? AnhDaiDienUrl { get; set; } // Link ảnh avatar

        // Dùng Enum môn học có sẵn để biết giáo viên này dạy môn gì
        public MonHocEnum MonHocChuyenMon { get; set; }

        public string? ThanhTichNoiBat { get; set; }

        public string? PhuongPhapGiangDay { get; set; }

        // Navigation property
        public virtual NguoiDung NguoiDung { get; set; } = null!;
    }
}

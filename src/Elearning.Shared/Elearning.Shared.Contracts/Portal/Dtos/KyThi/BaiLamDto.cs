using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos.KyThi
{
    public class BaiLamDto : BaseEntiyDto
    {
        public int STT { get; set; }

        // 1. ĐÃ SỬA: Nullable vì bài làm có thể thuộc BoCauHoiOnTap
        public Guid? KyThiId { get; set; }

        // BỔ SUNG: Tên kỳ thi để hiển thị ra UI
        public string? TenKyThi { get; set; }

        public Guid? BoCauHoiOnTapId { get; set; }
        public string? TenBoCauHoi { get; set; }

        public Guid NguoiDungId { get; set; }

        // Thông tin sinh viên
        public string TenSinhVien { get; set; } = string.Empty;

        public DateTime ThoiDiemBatDau { get; set; }
        public DateTime? ThoiDiemNop { get; set; }

        public float Diem { get; set; }
        public int SoCauDung { get; set; }

        // 2. BỔ SUNG: Tổng số câu để UI hiển thị "Đúng X / Y câu"
        public int TongSoCau { get; set; }

        public EnumTrangThaiBaiLam TrangThai { get; set; }

        // Cột quan trọng: Đếm số lần gian lận
        public int TongSoLanViPham { get; set; }

        // 3. BỔ SUNG: Các cờ phân loại đề thi để UI (Review) biết đường render
        public bool IsKyThiPublic { get; set; }
        public string? MonHoc { get; set; }
    }
}


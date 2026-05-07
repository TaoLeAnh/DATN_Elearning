using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos.KyThi
{
    public class BaiLamReviewDto
    {
        public Guid BaiLamId { get; set; }

        // 1. THÊM TRƯỜNG NÀY: Để nút "Quay lại danh sách" biết phải quay về Kỳ thi nào
        public Guid KyThiId { get; set; }

        public string TenSinhVien { get; set; } = string.Empty;
        public string MaSinhVien { get; set; } = string.Empty;
        public string TenKyThi { get; set; } = string.Empty;

        // ========================================================
        // 2. THÊM 2 TRƯỜNG NÀY: Để hiển thị Badge "Công khai" và "Môn học"
        // ========================================================
        public bool IsKyThiPublic { get; set; }
        public string? MonHoc { get; set; }
        // ========================================================

        public float Diem { get; set; }
        public int SoCauDung { get; set; }
        public int TongSoCau { get; set; }
        public DateTime ThoiDiemBatDau { get; set; }
        public DateTime? ThoiDiemNop { get; set; }

        public List<CauHoiReviewDto> DanhSachCauHoi { get; set; } = new();
    }

    // Chi tiết từng câu hỏi trong đề (Giữ nguyên của bạn)
    public class CauHoiReviewDto
    {
        public Guid CauHoiId { get; set; }
        public int ThuTu { get; set; }
        public EnumLoaiPhanThi PhanThi { get; set; }
        public string NoiDungCauHoi { get; set; } = string.Empty;
        public string? GiaiThich { get; set; } // Lời giải thích khi xem lại

        // Trạng thái đúng/sai tổng thể của câu này
        public bool IsCorrect { get; set; }

        // -- Dành cho Phần 1 (Trắc nghiệm 1 đáp án) --
        public List<DapAnReviewDto> DapAns { get; set; } = new();
        public Guid? DapAnHocVienChonId { get; set; } // SV chọn đáp án nào?

        // -- Dành cho Phần 2 (Mệnh đề Đúng/Sai) --
        public List<MenhDeReviewDto> MenhDes { get; set; } = new();

        // -- Dành cho Phần 3 (Điền kết quả) --
        public float? GiaTriHocVienNhap { get; set; }
        public float? GiaTriDung { get; set; }
        public float? SaiSoChoPhep { get; set; }
    }

    public class DapAnReviewDto
    {
        public Guid Id { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public bool LaDapAnDung { get; set; }
    }

    public class MenhDeReviewDto
    {
        public Guid Id { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public bool LaDung { get; set; } // Đáp án của hệ thống
        public bool? LuaChonCuaHocVien { get; set; } // SV chọn True hay False (Null nếu không tick)
    }
}

using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms.KyThi
{
    public class KyThiForm
    {
        [Required(ErrorMessage = "Tên kỳ thi không được để trống.")]
        [StringLength(200, ErrorMessage = "Tên kỳ thi không được vượt quá 200 ký tự.")]
        public string TenKyThi { get; set; } = default!;

        // Đổi thành Nullable (Không bắt buộc)
        public Guid? KhoaHocId { get; set; }

        // Đổi thành Nullable
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thời lượng kỳ thi.")]
        [Range(1, int.MaxValue, ErrorMessage = "Thời lượng phải lớn hơn 0.")]
        public int ThoiLuongPhut { get; set; }

        // --- CÁC TRƯỜNG MỚI THÊM VÀO ---
        public MonHocEnum? MonHoc { get; set; }
        public EnumLoaiDeThi? LoaiDeThi { get; set; }
        public int? NamThi { get; set; }

        [StringLength(100)]
        public string? TinhThanh { get; set; }

        [StringLength(255)]
        public string? TenTruong { get; set; }

        public bool IsPublic { get; set; } = true; // Mặc định là bật
    }
}

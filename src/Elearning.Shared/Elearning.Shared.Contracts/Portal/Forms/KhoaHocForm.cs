using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class KhoaHocForm
    {
        [Required(ErrorMessage = "Tên khóa học không được để trống.")]
        [StringLength(255, ErrorMessage = "Tên khóa học không được vượt quá 255 ký tự.")]
        public string TenKhoaHoc { get; set; } = default!;

        [Required(ErrorMessage = "Mô tả khóa học không được để trống.")]
        public string MoTa { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng chọn giảng viên phụ trách.")]
        public Guid GiangVienId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn môn học.")]
        public MonHocEnum MonHoc { get; set; }
        public string? HinhAnhUrl { get; set; }
        public decimal? GiaGoc { get; set; }
        public decimal? GiaBan { get; set; }
    }
}

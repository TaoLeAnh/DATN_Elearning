using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class ChuongHocForm
    {
        [Required(ErrorMessage = "Tên chương học không được để trống.")]
        [StringLength(255, ErrorMessage = "Tên chương học không được vượt quá 255 ký tự.")]
        public string TenChuong { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng chọn khóa học.")]
        public Guid KhoaHocId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thứ tự hiển thị của chương.")]
        public int ThuTu { get; set; }
    }
}

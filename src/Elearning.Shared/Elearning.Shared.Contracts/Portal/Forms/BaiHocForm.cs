using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class BaiHocForm
    {
        [Required(ErrorMessage = "Tiêu đề bài học không được để trống.")]
        [StringLength(300, ErrorMessage = "Tiêu đề không được vượt quá 300 ký tự.")]
        public string TieuDe { get; set; } = default!;

        public string? NoiDung { get; set; }

        [StringLength(500, ErrorMessage = "Đường dẫn Video không được vượt quá 500 ký tự.")]
        public string? VideoUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại bài học.")]
        public EnumLoaiBaiHoc Loai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thời lượng bài học.")]
        [Range(0, int.MaxValue, ErrorMessage = "Thời lượng phải là số dương.")]
        public int ThoiLuong { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chương học.")]
        public Guid ChuongHocId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thứ tự hiển thị.")]
        public int ThuTu { get; set; }
    }
}

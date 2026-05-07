using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class DangKyKhoaHocForm
    {
        [Required(ErrorMessage = "Vui lòng chọn người dùng.")]
        public Guid NguoiDungId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khóa học.")]
        public Guid KhoaHocId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái.")]
        public EnumTrangThaiDangKy TrangThai { get; set; } = EnumTrangThaiDangKy.ChoDuyet;

        public double TienDo { get; set; }
    }
}

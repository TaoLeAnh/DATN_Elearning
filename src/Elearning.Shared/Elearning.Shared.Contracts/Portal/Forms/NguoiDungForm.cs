using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class NguoiDungForm
    {
        [Required(ErrorMessage = "Tên người dùng không được để trống.")]
        [StringLength(255, ErrorMessage = "Tên không được vượt quá 255 ký tự.")]
        public string Ten { get; set; } = default!;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string MatKhau { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng chọn vai trò.")]
        public EnumVaiTro VaiTro { get; set; }
    }
}

using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class HoSoGiaoVienForm
    {
        [Required(ErrorMessage = "Vui lòng chọn giáo viên.")]
        public Guid NguoiDungId { get; set; }

        public string? AnhDaiDienUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn môn học chuyên môn.")]
        public MonHocEnum MonHocChuyenMon { get; set; }

        [MaxLength(2000, ErrorMessage = "Thành tích nổi bật không được vượt quá 2000 ký tự.")]
        public string? ThanhTichNoiBat { get; set; }

        [MaxLength(2000, ErrorMessage = "Phương pháp giảng dạy không được vượt quá 2000 ký tự.")]
        public string? PhuongPhapGiangDay { get; set; }
    }
}

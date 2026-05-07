using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class BoCauHoiOnTapForm
    {
        [Required(ErrorMessage = "Tên bộ câu hỏi không được để trống.")]
        [StringLength(200, ErrorMessage = "Tên không được vượt quá 200 ký tự.")]
        public string TenBo { get; set; } = default!;

        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại bộ câu hỏi.")]
        public EnumLoaiBoCauHoi LoaiBoCauHoi { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập thời lượng.")]
        public int ThoiLuongPhut { get; set; } = 45;
        public Guid? BaiHocId { get; set; }
        public Guid? ChuongHocId { get; set; }
        public Guid? KhoaHocId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giảng viên.")]
        public Guid GiangVienId { get; set; }

        // Danh sách ID câu hỏi được add vào bộ này
        public List<ChiTietBoCauHoiForm> ChiTietBoCauHois { get; set; } = new List<ChiTietBoCauHoiForm>();
    }
}

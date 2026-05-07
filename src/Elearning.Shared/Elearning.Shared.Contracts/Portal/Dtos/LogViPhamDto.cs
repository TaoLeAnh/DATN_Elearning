using Elearning.Shared.Commons.Extensions;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class LogViPhamDto : BaseEntiyDto
    {
        public int STT { get; set; }
        public Guid BaiLamId { get; set; }

        // Thông tin người dùng (Lấy qua bảng Bài Làm)
        public string TenNguoiDung { get; set; } = string.Empty;

        // Thông tin Kỳ thi/Đề thi
        public string TenDeThi { get; set; } = string.Empty;

        public EnumLoaiViPham LoaiViPham { get; set; }
        public string TenLoaiViPham => LoaiViPham.GetDescription(); // Extension method bạn đang xài

        public DateTime ThoiDiemViPham { get; set; }
        public string? ChiTiet { get; set; }
    }
}

using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class BoCauHoiOnTapDto : BaseEntiyDto
    {
        public int STT { get; set; }
        public string TenBo { get; set; } = default!;
        public string? MoTa { get; set; }
        public int ThoiLuongPhut { get; set; }
        public EnumLoaiBoCauHoi LoaiBoCauHoi { get; set; }

        public Guid? BaiHocId { get; set; }
        public string? TenBaiHoc { get; set; } // Hiển thị UI

        public Guid? ChuongHocId { get; set; }
        public string? TenChuongHoc { get; set; } // Hiển thị UI

        public Guid? KhoaHocId { get; set; }
        public string? TenKhoaHoc { get; set; }
        public Guid GiangVienId { get; set; }
        public string? TenGiangVien { get; set; } // Hiển thị UI

        // Danh sách các câu hỏi thuộc bộ này
        public List<ChiTietBoCauHoiDto> ChiTietBoCauHois { get; set; } = new List<ChiTietBoCauHoiDto>();

        public int SoLuongCauHoi { get; set; }

    }
}

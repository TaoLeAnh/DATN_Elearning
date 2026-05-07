using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class BaiHocDto : BaseEntiyDto
    {
        public int STT { get; set; }

        public string TieuDe { get; set; } = default!;

        public string? NoiDung { get; set; }

        public string? VideoUrl { get; set; }

        public int ThoiLuong { get; set; }

        public EnumLoaiBaiHoc Loai { get; set; }

        public Guid ChuongHocId { get; set; }

        // Hiển thị tên chương học thay vì chỉ ID
        public string? TenChuong { get; set; }

        public int ThuTu { get; set; }
        public List<BoCauHoiOnTapDto> DanhSachBoCauHoi { get; set; } = new List<BoCauHoiOnTapDto>();
    }
}

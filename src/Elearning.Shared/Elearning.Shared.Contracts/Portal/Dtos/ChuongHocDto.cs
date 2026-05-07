using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class ChuongHocDto : BaseEntiyDto
    {
        public int STT { get; set; }
        public string TenChuong { get; set; } = default!;

        public Guid KhoaHocId { get; set; }

        public string? TenKhoaHoc { get; set; }

        public int ThuTu { get; set; }
        public List<BaiHocDto> BaiHocs { get; set; } = new List<BaiHocDto>();
        public List<BoCauHoiOnTapDto> DanhSachBoCauHoi { get; set; } = new List<BoCauHoiOnTapDto>();
    }
}

using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class MyCourseDto : BaseEntiyDto
    {
        public int STT { get; set; }
        public string TenKhoaHoc { get; set; }
        public string? HinhAnhUrl { get; set; }
        public string? TenGiangVien { get; set; }
        public int TongSoBaiHoc { get; set; }
        public int SoBaiDaHoanThanh { get; set; }
        public double TienDo => TongSoBaiHoc == 0 ? 0 : Math.Round((double)SoBaiDaHoanThanh / TongSoBaiHoc * 100, 1);
        public DateTime? NgayDangKy { get; set; }
        public string? BaiHocCuoiCungTen { get; set; } // Để bấm "Học tiếp"
        public Guid? BaiHocCuoiCungId { get; set; }
        public Elearning.Shared.Contracts.Portal.Enums.MonHocEnum MonHoc { get; set; }
    }
}

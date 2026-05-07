using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class KhoaHocDto : BaseEntiyDto
    {
        public int STT { get; set; }

        public string TenKhoaHoc { get; set; } = default!;

        public string MoTa { get; set; } = default!;

        public Guid GiangVienId { get; set; }

        public MonHocEnum MonHoc { get; set; }

        public string? HinhAnhUrl { get; set; }
        public decimal? GiaGoc { get; set; }
        public decimal? GiaBan { get; set; }

        public string TenGiangVien { get; set; } = default!;
        public List<ChuongHocDto> ChuongHocs { get; set; } = new List<ChuongHocDto>();
        public List<BoCauHoiOnTapDto> DanhSachBoCauHoi { get; set; } = new List<BoCauHoiOnTapDto>();
        public int SoBaiHoc { get; set; } // Thêm trường này
    }
}


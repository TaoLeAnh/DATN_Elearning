using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class KhoaHoc : BaseDomainEntity
    {
        public string TenKhoaHoc { get; set; } = default!;

        public string MoTa { get; set; } = default!;
        public MonHocEnum MonHoc { get; set; }
        public Guid GiangVienId { get; set; }
        public string? HinhAnhUrl { get; set; } // Link ảnh bìa khóa học
        public decimal? GiaGoc { get; set; }    // Ví dụ: 1800000
        public decimal? GiaBan { get; set; }
        public virtual NguoiDung GiangVien { get; set; } = default!;

        public virtual ICollection<ChuongHoc> ChuongHocs { get; set; } = new List<ChuongHoc>();

        public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();

        public virtual ICollection<KyThi> KyThis { get; set; } = new List<KyThi>();
    }
}

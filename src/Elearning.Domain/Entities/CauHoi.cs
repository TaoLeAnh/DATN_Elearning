using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class CauHoi : BaseDomainEntity
    {
        public string? NoiDung { get; set; }
        public string? HinhAnhUrl { get; set; }
        public EnumLoaiCauHoi LoaiCauHoi { get; set; }

        public EnumMucDo MucDo { get; set; } 

        public string ChuDe { get; set; } = default!;

        public string? GiaiThich { get; set; }
        public MonHocEnum? MonHoc { get; set; }

        public Guid GiangVienId { get; set; }

        public virtual NguoiDung GiangVien { get; set; } = default!;

        public Guid? KhoaHocId { get; set; }
        public virtual KhoaHoc KhoaHoc { get; set; } = default!;

        public virtual ICollection<DapAn> DapAns { get; set; } = new List<DapAn>();
        public virtual ICollection<MenhDeDungSai> MenhDeDungSais { get; set; } = new List<MenhDeDungSai>();

        public virtual ICollection<DapAnDienKetQua> DapAnDienKetQuas { get; set; } = new List<DapAnDienKetQua>();
        public virtual ICollection<CauHoiKyThi> CauHoiKyThis { get; set; } = new List<CauHoiKyThi>();

        public virtual ICollection<ChiTietBoCauHoi> ChiTietBoCauHois { get; set; } = new List<ChiTietBoCauHoi>();

        public virtual ICollection<ChiTietBaiLam> ChiTietBaiLams { get; set; } = new List<ChiTietBaiLam>();
    }
}

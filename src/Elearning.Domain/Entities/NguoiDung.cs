using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class NguoiDung : BaseDomainEntity
    {
        public string Ten { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string? MaHocSinh { get; set; }

        public string MatKhau { get; set; } = default!;

        public EnumVaiTro VaiTro { get; set; }

        // Navigation

        public virtual ICollection<KhoaHoc> KhoaHocGiangDays { get; set; } = new List<KhoaHoc>();

        public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();

        public virtual ICollection<TienDoHoc> TienDoHocs { get; set; } = new List<TienDoHoc>();

        public virtual ICollection<BaiLam> BaiLams { get; set; } = new List<BaiLam>();

        public virtual ICollection<CauHoi> CauHois { get; set; } = new List<CauHoi>();

        public virtual ICollection<BoCauHoiOnTap> BoCauHoiOnTaps { get; set; } = new List<BoCauHoiOnTap>();

        public virtual HoSoGiaoVien? HoSoGiaoVien { get; set; }
    }
}

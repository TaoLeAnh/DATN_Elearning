using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class BoCauHoiOnTap : BaseDomainEntity
    {
        public string TenBo { get; set; } = default!;

        public string? MoTa { get; set; }
        public int ThoiLuongPhut { get; set; } = 45;
        public EnumLoaiBoCauHoi LoaiBoCauHoi { get; set; }

        public Guid? BaiHocId { get; set; }
        public virtual BaiHoc? BaiHoc { get; set; }

        public Guid? ChuongHocId { get; set; }
        public virtual ChuongHoc? ChuongHoc { get; set; }

        public Guid? KhoaHocId { get; set; }
        public virtual KhoaHoc? KhoaHoc { get; set; }
        public Guid GiangVienId { get; set; }

        public virtual NguoiDung GiangVien { get; set; } = default!;

        public virtual ICollection<ChiTietBoCauHoi> ChiTietBoCauHois { get; set; } = new List<ChiTietBoCauHoi>();
    }
}

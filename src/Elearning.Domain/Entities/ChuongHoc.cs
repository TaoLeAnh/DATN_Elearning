using Elearning.Shared.Commons.Model.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class ChuongHoc : BaseDomainEntity
    {
        public string TenChuong { get; set; } = default!;

        public Guid KhoaHocId { get; set; }

        public int ThuTu { get; set; }

        public virtual KhoaHoc KhoaHoc { get; set; } = default!;

        public virtual ICollection<BaiHoc> BaiHocs { get; set; } = new List<BaiHoc>();
    }
}

using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class CauHoiKyThi : BaseDomainEntity
    {
        public Guid KyThiId { get; set; }

        public Guid CauHoiId { get; set; }

        public EnumLoaiPhanThi PhanThi { get; set; }

        public int ThuTu { get; set; }

        public virtual KyThi KyThi { get; set; } = default!;

        public virtual CauHoi CauHoi { get; set; } = default!;
    }
}

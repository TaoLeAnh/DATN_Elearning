using Elearning.Shared.Commons.Model.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class DapAnDienKetQua : BaseDomainEntity
    {
        public Guid CauHoiId { get; set; }

        public float GiaTriDung { get; set; }

        public float SaiSoChoPhep { get; set; }

        public virtual CauHoi CauHoi { get; set; } = default!;
    }
}

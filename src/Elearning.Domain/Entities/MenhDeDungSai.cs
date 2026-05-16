using Elearning.Shared.Commons.Model.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class MenhDeDungSai : BaseDomainEntity
    {
        public Guid CauHoiId { get; set; }

        public string? HinhAnhUrl { get; set; }

        public string? NoiDung { get; set; } = default!;

        public bool LaDung { get; set; }

        public int ThuTu { get; set; }

        public virtual CauHoi CauHoi { get; set; } = default!;
        public virtual ICollection<ChiTietTraLoiMenhDe> ChiTietTraLoiMenhDes { get; set; } = new List<ChiTietTraLoiMenhDe>();
    }
}

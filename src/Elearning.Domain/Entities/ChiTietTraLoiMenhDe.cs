using Elearning.Shared.Commons.Model.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class ChiTietTraLoiMenhDe : BaseDomainEntity
    {
        public Guid ChiTietBaiLamId { get; set; }
        public Guid MenhDeDungSaiId { get; set; }
        public bool LuaChonCuaHocVien { get; set; } // Học viên chọn True (Đúng) hay False (Sai)

        public virtual ChiTietBaiLam ChiTietBaiLam { get; set; } = default!;
        public virtual MenhDeDungSai MenhDeDungSai { get; set; } = default!;
    }
}

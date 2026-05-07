using Elearning.Shared.Commons.Model.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class ChiTietBaiLam : BaseDomainEntity
    {
        public Guid BaiLamId { get; set; }

        public Guid CauHoiId { get; set; }

        public Guid? DapAnId { get; set; }

        public float? GiaTriNhap { get; set; }

        public virtual BaiLam BaiLam { get; set; } = default!;

        public virtual CauHoi CauHoi { get; set; } = default!;

        public virtual DapAn? DapAn { get; set; }

        // THÊM: List này để lưu 4 lựa chọn Đúng/Sai của sinh viên cho câu hỏi Phần 2
        public virtual ICollection<ChiTietTraLoiMenhDe> ChiTietTraLoiMenhDes { get; set; } = new List<ChiTietTraLoiMenhDe>();
    }
}

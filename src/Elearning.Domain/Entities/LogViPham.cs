using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class LogViPham : BaseDomainEntity
    {
        public Guid BaiLamId { get; set; }

        public EnumLoaiViPham LoaiViPham { get; set; }

        // Ghi nhận chính xác giờ phút giây sinh viên vi phạm
        public DateTime ThoiDiemViPham { get; set; }

        // Ghi chú thêm (VD: "Sinh viên rời khỏi tab trong 15 giây")
        public string? ChiTiet { get; set; }

        public virtual BaiLam BaiLam { get; set; } = default!;
    }
}

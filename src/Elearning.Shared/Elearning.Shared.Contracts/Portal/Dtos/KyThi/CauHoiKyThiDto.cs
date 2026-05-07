using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos.KyThi
{
    public class CauHoiKyThiDto
    {
        public Guid Id { get; set; }
        public Guid KyThiId { get; set; }
        public Guid CauHoiId { get; set; }
        public EnumLoaiPhanThi PhanThi { get; set; }
        public int ThuTu { get; set; }

        // Kéo nội dung từ bảng CauHoi lên để UI hiển thị
        public string? NoiDungCauHoi { get; set; }
    }
}

using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class TienDoHocDto : BaseEntiyDto
    {
        public int STT { get; set; }

        public Guid NguoiDungId { get; set; }
        public string? TenNguoiDung { get; set; }

        public Guid BaiHocId { get; set; }
        public string? TieuDeBaiHoc { get; set; }

        public bool DaHoanThanh { get; set; }

        public DateTime? ThoiDiemHoanThanh { get; set; }

        public int LastTimePosition { get; set; }
    }
}

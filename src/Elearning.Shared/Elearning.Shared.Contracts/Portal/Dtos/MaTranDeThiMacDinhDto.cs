using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class MaTranDeThiMacDinhDto : BaseEntiyDto
    {

        public int STT { get; set; }
        public MonHocEnum MonHoc { get; set; }
        public string TenMaTran { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int TongSoCau { get; set; } // Thuộc tính tính toán thêm để hiển thị lưới

        public List<ChiTietMaTranMacDinhDto> ChiTiets { get; set; } = new();
    }
}

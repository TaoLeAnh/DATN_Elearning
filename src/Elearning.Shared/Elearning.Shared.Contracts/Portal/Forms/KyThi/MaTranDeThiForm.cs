using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms.KyThi
{
    public class MaTranDeThiForm
    {
        // Khóa học ID để giới hạn chỉ bốc câu hỏi của môn đó (Tránh bốc lộn câu Toán sang đề Hóa)
        public Guid KhoaHocId { get; set; }

        // Danh sách các "Luật" bốc câu hỏi
        public List<LuatRandomForm> DanhSachLuat { get; set; } = new();
    }

    public class LuatRandomForm
    {
        public EnumLoaiPhanThi PhanThi { get; set; } // Bốc cho Phần 1, 2 hay 3?
        public EnumLoaiCauHoi LoaiCauHoiGoc { get; set; } // (Trắc nghiệm 1ĐA, Đúng/Sai, Điền số)
        public string ChuDe { get; set; } = string.Empty; // "Đại số", "Hình học", "Hữu cơ"...
        public EnumMucDo MucDo { get; set; } // Dễ, TB, Khó
        public int SoLuongCanLay { get; set; }
    }
}

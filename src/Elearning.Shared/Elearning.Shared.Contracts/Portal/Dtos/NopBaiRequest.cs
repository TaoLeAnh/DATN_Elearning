using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class NopBaiRequest
    {
        public Guid BaiLamId { get; set; }
        public Guid BoCauHoiId { get; set; } // ID của bộ đề đang làm
        public List<CauTraLoiRequest> DanhSachTraLoi { get; set; } = new List<CauTraLoiRequest>();
        public Guid UserId { get; set; }
        public int ThoiGianLamBaiGiay { get; set; }
        public bool IsLiveExam { get; set; }
    }

    public class CauTraLoiRequest
    {
        public Guid CauHoiId { get; set; }

        // Dùng cho Loại 1 (Trắc nghiệm A,B,C,D)
        public Guid? DapAnId { get; set; }

        // Dùng cho Loại 3 (Điền kết quả)
        public float? GiaTriNhap { get; set; }

        // Dùng cho Loại 2 (Đúng/Sai nhiều ý)
        public List<MenhDeTraLoiRequest> MenhDes { get; set; } = new List<MenhDeTraLoiRequest>();
    }

    public class MenhDeTraLoiRequest
    {
        public Guid MenhDeId { get; set; }
        public bool LuaChonCuaHocVien { get; set; }
    }
}

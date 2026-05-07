using System.ComponentModel;

namespace Elearning.Shared.Contracts.Portal.Enums
{
    public enum EnumLoaiPhanThi
    {
        [Description("Phần 1: Trắc nghiệm nhiều lựa chọn")]
        TracNghiem = 1,

        [Description("Phần 2: Trắc nghiệm Đúng/Sai")]
        MenhDeDungSai = 2,

        [Description("Phần 3: Trắc nghiệm trả lời ngắn (Điền kết quả)")]
        DienKetQua = 3
    }
}

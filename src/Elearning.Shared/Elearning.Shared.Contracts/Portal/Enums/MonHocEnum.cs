using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Enums
{
    public enum MonHocEnum
    {
        [Description("Môn Toán")]
        Toan = 1,
        [Description("Môn Ngữ Văn")]
        NguVan = 2,
        [Description("Môn Tiếng Anh")]
        TiengAnh = 3,
        [Description("Môn Vật Lý")]
        VatLy = 4,
        [Description("Môn Hóa Học")]
        HoaHoc = 5,
        [Description("Môn Sinh Học")]
        SinhHoc = 6,
        [Description("Môn Lịch Sử")]
        LichSu = 7,
        [Description("Môn Địa Lý")]
        DiaLy = 8,
        [Description("Môn Giáo Dục Công Dân")]
        GDCD = 9,
        [Description("Môn Khoa học và đọc hiểu")]
        KhoaHocVaDocHieu = 10
    }
}

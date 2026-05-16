using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class CauHoiForm
    {
        public string? NoiDung { get; set; }

        [StringLength(500, ErrorMessage = "Đường dẫn ảnh không được vượt quá 500 ký tự.")]
        public string? HinhAnhUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại câu hỏi.")]
        public EnumLoaiCauHoi LoaiCauHoi { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn mức độ khó.")]
        public EnumMucDo MucDo { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chủ đề.")]
        [StringLength(200, ErrorMessage = "Chủ đề không được vượt quá 200 ký tự.")]
        public string ChuDe { get; set; } = default!;

        public Guid? KhoaHocId { get; set; }

        // --- BỔ SUNG TRƯỜNG MÔN HỌC ---
        public MonHocEnum? MonHoc { get; set; }

        public string? GiaiThich { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giảng viên.")]
        public Guid GiangVienId { get; set; }

        public List<DapAnForm> DapAns { get; set; } = new List<DapAnForm>();
        public List<MenhDeDungSaiForm> MenhDeDungSais { get; set; } = new();
        public List<DapAnDienKetQuaForm> DapAnDienKetQuas { get; set; } = new();
    }
    public class MenhDeDungSaiForm
    {
        public string? HinhAnhUrl { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public bool LaDung { get; set; }
        public int ThuTu { get; set; }
    }

    public class DapAnDienKetQuaForm
    {
        public float GiaTriDung { get; set; }
        public float SaiSoChoPhep { get; set; }
    }
}

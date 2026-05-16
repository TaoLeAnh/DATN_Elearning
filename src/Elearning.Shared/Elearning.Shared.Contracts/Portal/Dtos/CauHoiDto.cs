using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class CauHoiDto : BaseEntiyDto
    {
        public int STT { get; set; }

        public string? NoiDung { get; set; }

        public string? HinhAnhUrl { get; set; }

        public EnumLoaiCauHoi LoaiCauHoi { get; set; }

        public EnumMucDo MucDo { get; set; }

        public Guid? KhoaHocId { get; set; }
        public MonHocEnum? MonHoc { get; set; }
        public string ChuDe { get; set; } = default!;

        public string? GiaiThich { get; set; }

        public Guid GiangVienId { get; set; }

        // Hiển thị tên giảng viên trên Grid
        public string? TenGiangVien { get; set; }

        // Danh sách đáp án đi kèm câu hỏi này (dùng khi xem chi tiết)
        public List<DapAnDto> DapAns { get; set; } = new List<DapAnDto>();
        public List<MenhDeDungSaiDto> MenhDeDungSais { get; set; } = new();
        public List<DapAnDienKetQuaDto> DapAnDienKetQuas { get; set; } = new();
    }
    public class MenhDeDungSaiDto
    {
        public Guid Id { get; set; }
        public string? HinhAnhUrl { get; set; }
        public string NoiDung { get; set; }
        public bool LaDung { get; set; }
        public int ThuTu { get; set; }
    }

    public class DapAnDienKetQuaDto
    {
        public Guid Id { get; set; }
        public float GiaTriDung { get; set; }
        public float SaiSoChoPhep { get; set; }
    }
}

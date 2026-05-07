using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class ChiTietBoCauHoiDto : BaseEntiyDto
    {
        public Guid BoCauHoiOnTapId { get; set; }
        public Guid CauHoiId { get; set; }
        public int ThuTu { get; set; }

        // Kéo thêm nội dung câu hỏi để hiển thị lúc xem chi tiết Đề thi
        public string? NoiDungCauHoi { get; set; }
        public string? HinhAnhUrlCauHoi { get; set; }
        public EnumLoaiCauHoi LoaiCauHoi { get; set; }

        public List<DapAnDto> DapAns { get; set; } = new List<DapAnDto>();
        public List<MenhDeDungSaiDto> MenhDeDungSais { get; set; } = new List<MenhDeDungSaiDto>();
        public List<DapAnDienKetQuaDto> DapAnDienKetQuas { get; set; } = new List<DapAnDienKetQuaDto>();
    }
    public class MenhDeDungSaiDto : BaseEntiyDto
    { 
        public string NoiDung { get; set; } = default!;
        public int ThuTu { get; set; } 
    }
    public class DapAnDienKetQuaDto : BaseEntiyDto
    {
    }
}

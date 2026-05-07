using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class DapAnDto : BaseEntiyDto
    {
        public Guid CauHoiId { get; set; }

        public string? NoiDung { get; set; }

        public string? HinhAnhUrl { get; set; }

        public bool LaDapAnDung { get; set; }

        public int ThuTu { get; set; }
    }
}

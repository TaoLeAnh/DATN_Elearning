using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class NguoiDungDto : BaseEntiyDto
    {
        public int STT { get;set; }
        public string Ten { get; set; } = default!;

        public string Email { get; set; } = default!;

        public EnumVaiTro VaiTro { get; set; }
    }
}

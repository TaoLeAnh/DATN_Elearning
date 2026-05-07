using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class SystemParameterDto : BaseEntiyDto
    {
        public int STT { get; set; }
        public string Code { get; set; } = "";
        public string? Value { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// Đồng bộ bằng enum
        /// </summary>
        public bool IsSync { get; set; }
    }
    public class SystemParameterCodeDto
    {
        public List<string> Codes { get; set; } = new();
    }
}

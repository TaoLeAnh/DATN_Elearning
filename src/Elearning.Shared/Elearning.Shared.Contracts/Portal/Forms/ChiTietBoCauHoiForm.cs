using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class ChiTietBoCauHoiForm
    {
        [Required]
        public Guid CauHoiId { get; set; }
        public int ThuTu { get; set; }
    }
}

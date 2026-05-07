using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class TienDoHocForm
    {
        [Required]
        public Guid NguoiDungId { get; set; }

        [Required]
        public Guid BaiHocId { get; set; }

        public bool DaHoanThanh { get; set; }

        public DateTime? ThoiDiemHoanThanh { get; set; }

        public int LastTimePosition { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Forms
{
    public class DapAnForm
    {
        public string? NoiDung { get; set; }

        [StringLength(500, ErrorMessage = "Đường dẫn ảnh không được vượt quá 500 ký tự.")]
        public string? HinhAnhUrl { get; set; }

        public bool LaDapAnDung { get; set; }

        public int ThuTu { get; set; }
    }
}

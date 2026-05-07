using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class BaiHoc : BaseDomainEntity
    {
        public string TieuDe { get; set; } = default!;

        // Chứa nội dung văn bản nếu là bài đọc
        public string? NoiDung { get; set; }

        // Link Youtube, Vimeo hoặc lưu trữ riêng
        public string? VideoUrl { get; set; }

        // Thời lượng bài học tính bằng phút
        public int ThoiLuong { get; set; }

        public Guid ChuongHocId { get; set; }

        public int ThuTu { get; set; }

         public EnumLoaiBaiHoc Loai { get; set; }

        public virtual ChuongHoc ChuongHoc { get; set; } = default!;
    }
}

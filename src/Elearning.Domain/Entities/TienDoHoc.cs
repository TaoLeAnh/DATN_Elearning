using Elearning.Shared.Commons.Model.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class TienDoHoc : BaseDomainEntity
    {
        public Guid NguoiDungId { get; set; }

        public Guid BaiHocId { get; set; }

        // Đánh dấu đã học xong bài này chưa
        public bool DaHoanThanh { get; set; }

        // Thời điểm bấm "Hoàn thành"
        public DateTime? ThoiDiemHoanThanh { get; set; }

        // Thời gian xem video gần nhất (tính bằng giây) 
        // Dùng để làm tính năng "Học tiếp" (Resume)
        public int LastTimePosition { get; set; }

        // Quan hệ Navigation
        public virtual NguoiDung NguoiDung { get; set; } = default!;

        public virtual BaiHoc BaiHoc { get; set; } = default!;
    }
}

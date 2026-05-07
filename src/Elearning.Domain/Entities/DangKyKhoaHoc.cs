using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class DangKyKhoaHoc : BaseDomainEntity
    {
        public Guid NguoiDungId { get; set; }

        public Guid KhoaHocId { get; set; }

        // Thời điểm đăng ký thành công
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        // Trạng thái: 0-Chờ duyệt, 1-Đang học, 2-Hoàn thành, 3-Hủy
        public EnumTrangThaiDangKy TrangThai { get; set; }

        // Lưu % tiến độ (ví dụ 10.5%). 
        // Được tính toán lại mỗi khi hoàn thành 1 bài học
        public double TienDo { get; set; }

        // Quan hệ Navigation
        public virtual NguoiDung NguoiDung { get; set; } = default!;

        public virtual KhoaHoc KhoaHoc { get; set; } = default!;
    }
}

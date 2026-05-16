using Elearning.Shared.Commons.Querys.ModalQuery;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Querys.KyThi
{
    public class BaiLamQuery : BaseQuery
    {
        // Giảng viên phải truyền ID của kỳ thi vào đây để xem danh sách lớp
        public Guid KyThiId { get; set; }

        // Tuỳ chọn: Lọc theo trạng thái (Ví dụ chỉ xem những đứa đã nộp)
        public int? TrangThai { get; set; }
        public Guid? NguoiDungId { get; set; }
    }
}

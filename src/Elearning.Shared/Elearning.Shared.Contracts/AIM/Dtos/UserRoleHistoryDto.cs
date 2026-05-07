using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class UserRoleHistoryDto : BaseEntiyDto
    {
        public int STT { get; set; }
        public List<string>? ListRoleBeforeEdit { get; set; }
        public List<string>? ListRoleAfterEdit { get; set; }
        public Guid UserId { get; set; }
        public Guid ChuyenTrangId { get; set; }
        public string ChuyenTrangName { get; set; } = string.Empty;
        public List<string>? LstChuyenMucAfterEdit { get; set; }
        public string? TenNguoiThucHien { get; set; }
        public List<string>? LstChuyenMucBeforeEdit { get; set; }

    }
}

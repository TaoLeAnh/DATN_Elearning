using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class RoleDto : BaseEntiyDto
    {
        public int STT { get; set; }
        public string RoleName { set; get; } = string.Empty;
        public string RoleCode { set; get; } = string.Empty;
        public string? Mota { set; get; } = string.Empty;
        public bool IsSync { get; set; }
        public bool DaGan { get; set; } = false;

        public List<Guid>? IdPermissions { get; set; }

        public int CountPermission { get; set; }
        public int CountMenu { get; set; }

    }
    public class GanQuyenDto
    {
        public List<Guid>? LstIdPermission { get; set; }
    }

    public class GanVaiTroVaoNguoiDungDto
    {
        public Guid UserId { get; set; }
        public Guid PhongBanId { get; set; }
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
        public List<Guid> PermissionIds { get; set; } = new List<Guid>();
        public List<Guid>? LstUserIds { get; set; }

        public List<Guid>? ChuyenTrangIds { get; set; }
    }
}

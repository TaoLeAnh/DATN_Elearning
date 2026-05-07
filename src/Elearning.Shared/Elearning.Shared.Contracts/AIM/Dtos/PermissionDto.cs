using Elearning.Shared.Commons.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class PermissionDto : BaseEntiyDto
    {
        public int STT { get; set; }
        public string PermissionName { set; get; } = string.Empty;
        public EnumPermissions PermissionCode { set; get; }
        public string? Description { set; get; } = string.Empty;
        public bool IsSync { get; set; }
        public string? PermissionParentName { get; set; }
        public bool IsSelected { get; set; }

        // Parent-Child
        public Guid? ParentId { get; set; }
        public List<PermissionDto>? ListChild { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class RoleModuleDto
    {
        public Guid ModuleId { get; set; }
        public virtual ModuleDto Module { get; set; } = null!;

        public Guid RoleId { get; set; }
        public virtual RoleDto? Role { get; set; } = null!;

    }
    public class GanMenuVaoVaiTroDto
    {
        public Guid RoleId { get; set; }
        public List<Guid> ModuleIds { get; set; } = new List<Guid>();

    }
}

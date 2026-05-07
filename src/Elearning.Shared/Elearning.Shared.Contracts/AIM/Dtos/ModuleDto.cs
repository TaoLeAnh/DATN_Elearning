using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.AIM.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class ModuleDto
    {
        public Guid Id { get; set; }
        public EnumModuleType PhanLoai { get; set; }
        public string TenModule { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? LienKet { get; set; }
        public bool Expands { get; set; } = false;
        public int ViTri { get; set; }
        public Guid? ModuleChaId { get; set; }
        public ModuleDto? ModuleCha { get; set; }
        public List<ModuleDto> Children { get; set; } = new();
        public ModerationStatus ModerationStatus { get; set; }
        public bool DaGan { get; set; } = false;
    }
}

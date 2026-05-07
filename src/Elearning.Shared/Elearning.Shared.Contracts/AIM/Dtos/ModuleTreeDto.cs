using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.AIM.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class ModuleTreeDto
    {
        public Guid Id { get; set; }
        public Guid? ModuleChaId { set; get; }
        public string TenModule { set; get; } = string.Empty;
        public string? Icon { get; set; }
        public string? LienKet { get; set; }
        public bool Expands { get; set; } = false;
        public int ViTri { get; set; }
        public EnumModuleType PhanLoaiMenu { get; set; } = EnumModuleType.KhongXacDinh;
        public EnumModuleType PhanLoai { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ModuleTreeDto> Children { get; set; } = new List<ModuleTreeDto>();
        public ModerationStatus ModerationStatus { get; set; }

        public bool Checked { get; set; } = false;
    }
}

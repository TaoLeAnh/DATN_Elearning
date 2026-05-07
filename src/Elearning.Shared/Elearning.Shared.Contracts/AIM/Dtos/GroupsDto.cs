using Elearning.Shared.Contracts.AIM.Enums;
using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class GroupsDto : BaseEntiyDto
    {
        public int STT { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortName { get; set; }// Tên viết tắt
        public string? IdentifierCode { get; set; }             // Mã định danh
        public string? Description { get; set; }
        public string? Avartar { get; set; }
        public OrganizationUnitType UnitType { get; set; }//Phan loai don vi hay phong ban
        public OrganizationUnitGroup UnitGroup { get; set; } //Nhóm đơn vị
        public int SortOrder { get; set; } = 999;
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TenDonViTrucThuoc { get; set; }
        public Guid? ParentId { get; set; }
        public List<GroupsDto>? ListChild { get; set; }
    }
}

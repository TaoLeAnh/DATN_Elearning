using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class MaTranDeThiMacDinh : BaseDomainEntity
    {
        public MonHocEnum MonHoc { get; set; }

        public string TenMaTran { get; set; } = string.Empty; 

        public bool IsActive { get; set; } 

        // Navigation
        public virtual ICollection<ChiTietMaTranMacDinh> ChiTiets { get; set; } = new List<ChiTietMaTranMacDinh>();
    }
}

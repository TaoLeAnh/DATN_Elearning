using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface IBaiHocService
    {
        Task<DataTableJson> GetPaged(BaiHocQuery searchOption);
        Task<BaiHocDto> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(BaiHocForm form);
        Task<bool> UpdateAsync(Guid id, BaiHocForm item);
        Task<bool> DeleteAsync(Guid id);
    }
}

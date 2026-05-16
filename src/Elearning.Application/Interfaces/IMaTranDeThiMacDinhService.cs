using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface IMaTranDeThiMacDinhService
    {
        Task<DataTableJson> GetPaged(MaTranDeThiMacDinhQuery searchOption);
        Task<MaTranDeThiMacDinhDto> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(MaTranDeThiMacDinhForm form);
        Task<bool> UpdateAsync(Guid id, MaTranDeThiMacDinhForm item);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);
        Task<List<MaTranDeThiMacDinhDto>> GetActiveByKyThiIdAsync(Guid kyThiId);
    }
}

using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface ITienDoHocService
    {
        Task<DataTableJson> GetPaged(TienDoHocQuery searchOption);
        Task<TienDoHocDto> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(TienDoHocForm form);
        Task<bool> UpdateAsync(Guid id, TienDoHocForm item);
        Task<bool> DeleteAsync(Guid id);
    }
}

using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface IBoCauHoiOnTapService
    {
        Task<DataTableJson> GetPaged(BoCauHoiOnTapQuery searchOption);
        Task<BoCauHoiOnTapDto> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(BoCauHoiOnTapForm form);
        Task<bool> UpdateAsync(Guid id, BoCauHoiOnTapForm item);
        Task<bool> DeleteAsync(Guid id);
    }
}

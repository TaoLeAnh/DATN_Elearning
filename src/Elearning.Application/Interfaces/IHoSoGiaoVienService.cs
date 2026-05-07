using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface IHoSoGiaoVienService
    {
        Task<DataTableJson> GetPaged(HoSoGiaoVienQuery searchOption);
        Task<HoSoGiaoVienDto> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(HoSoGiaoVienForm form);
        Task<bool> UpdateAsync(Guid id, HoSoGiaoVienForm item);
        Task<bool> DeleteAsync(Guid id);
    }
}

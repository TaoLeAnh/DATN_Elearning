using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface ICauHoiService
    {
        Task<DataTableJson> GetPaged(CauHoiQuery searchOption);
        Task<CauHoiDto> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CauHoiForm form);
        Task<bool> UpdateAsync(Guid id, CauHoiForm item);
        Task<bool> DeleteAsync(Guid id);

        Task<List<string>> GetDanhSachChuDeTheoKyThiAsync(Guid kyThiId);
    }
}

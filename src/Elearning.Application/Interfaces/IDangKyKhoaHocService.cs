using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface IDangKyKhoaHocService
    {
        Task<DataTableJson> GetPaged(DangKyKhoaHocQuery searchOption);
        Task<DangKyKhoaHocDto> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(DangKyKhoaHocForm form);
        Task<bool> UpdateAsync(Guid id, DangKyKhoaHocForm item);
        Task<bool> DeleteAsync(Guid id);
    }
}

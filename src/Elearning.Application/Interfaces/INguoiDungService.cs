using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;

namespace Elearning.Application.Interfaces
{
    public interface INguoiDungService
    {
        Task<DataTableJson> GetPaged(NguoiDungQuery searchOption);
        Task<NguoiDungDto> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(NguoiDungForm form);
        Task<bool> UpdateAsync(Guid id, NguoiDungForm item);
        Task<bool> DeleteAsync(Guid Id);
    }
}

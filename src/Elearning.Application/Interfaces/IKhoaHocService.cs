using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;

namespace Elearning.Application.Interfaces
{
    public interface IKhoaHocService
    {
        /// <summary>
        /// Get danh sách phân trang
        /// </summary>
        Task<DataTableJson> GetPaged(KhoaHocQuery baseQuery);

        /// <summary>
        /// Get chi tiết theo ID
        /// </summary>
        Task<KhoaHocDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Tạo mới khóa học
        /// </summary>
        Task<Guid> CreateAsync(KhoaHocForm form);

        /// <summary>
        /// Cập nhật khóa học
        /// </summary>
        Task<bool> UpdateAsync(Guid id, KhoaHocForm form);

        /// <summary>
        /// Xóa khóa học
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        Task<List<KhoaHocDto>> GetAllAsync();
    }
}

using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;

namespace Elearning.Application.Interfaces
{
    public interface IChuongHocService
    {
        /// <summary>
        /// Get danh sách phân trang
        /// </summary>
        Task<DataTableJson> GetPaged(ChuongHocQuery baseQuery);

        /// <summary>
        /// Get chi tiết theo ID
        /// </summary>
        Task<ChuongHocDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Tạo mới khóa học
        /// </summary>
        Task<Guid> CreateAsync(ChuongHocForm form);

        /// <summary>
        /// Cập nhật khóa học
        /// </summary>
        Task<bool> UpdateAsync(Guid id, ChuongHocForm form);

        /// <summary>
        /// Xóa khóa học
        /// </summary>
        Task<bool> DeleteAsync(Guid id);
    }
}

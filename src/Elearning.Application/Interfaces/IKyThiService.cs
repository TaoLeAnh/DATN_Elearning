using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Forms.KyThi;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface IKyThiService
    {
        Task<DataTableJson> GetPaged(KyThiQuery searchOption);

        Task<KyThiDto> GetByIdAsync(Guid id);

        Task<Guid> CreateAsync(KyThiForm form);

        Task<bool> UpdateAsync(Guid id, KyThiForm form);

        Task<bool> DeleteAsync(Guid id);

        Task<List<CauHoiKyThiDto>> GetCauHinhDeThiAsync(Guid kyThiId);
        Task<bool> SaveCauHinhDeThiAsync(Guid kyThiId, CauHinhDeThiForm form);

        Task<bool> GenerateRandomExamAsync(Guid kyThiId, MaTranDeThiForm maTran);
        Task<bool> GenerateRandomExamTheoMaTranAsync(Guid kyThiId, Guid maTranId);
        Task<bool> GenerateRandomExamAsyncV2(Guid kyThiId, MaTranDeThiForm maTran);
    }
}

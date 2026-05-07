using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface IBaiLamService
    {
        Task<DataTableJson> GetPagedAdminAsync(BaiLamQuery searchOption);
        Task<BaiLamReviewDto> GetChiTietBaiLamAsync(Guid baiLamId);
    }
}

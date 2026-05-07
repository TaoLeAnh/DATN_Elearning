using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Interfaces
{
    public interface IKhoaHocService
    {
        Task<DataTableJson> GetPaged(KhoaHocQuery baseQuery);
        Task<KhoaHocDto> GetByIdAsync(Guid id);
        Task<KhoaHocDto> GetBySlugAsync(string slug);

        Task<KhoaHocDto> GetDetailByIdAsync(Guid id);
    }
}

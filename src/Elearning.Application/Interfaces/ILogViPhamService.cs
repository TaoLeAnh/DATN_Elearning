using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface ILogViPhamService
    {
        Task<DataTableJson> GetPaged(LogViPhamQuery searchOption);
        Task<LogViPhamDto> GetByIdAsync(Guid id);
    }
}

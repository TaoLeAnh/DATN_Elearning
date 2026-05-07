using Elearning.Domain.Entities;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Interfaces.MSSQL
{
    public interface ILogViPhamRepository : IRepository<LogViPham>
    {
        Task<(List<LogViPhamDto> Items, int Total)> GetPagedDtoAsync(LogViPhamQuery searchOption);
    }
}

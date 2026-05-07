using Elearning.Domain.Entities;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Querys;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Interfaces.MSSQL
{
    public interface IBaiLamRepository : IRepository<BaiLam>
    {
        Task<(List<BaiLamDto> Items, int Total)> GetPagedDtoAsync(BaiLamQuery searchOption);
    }
}

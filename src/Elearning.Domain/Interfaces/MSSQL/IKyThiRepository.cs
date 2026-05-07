using Elearning.Domain.Entities;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Interfaces.MSSQL
{
    public interface IKyThiRepository : IRepository<KyThi>
    {
        Task<(List<KyThiDto> Items, int Total)> GetPagedDtoAsync(KyThiQuery searchOption);
    }
}

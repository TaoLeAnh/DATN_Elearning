using Elearning.Domain.Entities;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Interfaces.MSSQL
{
    public interface ICauHoiRepository : IRepository<CauHoi>
    {
        Task<(List<CauHoiDto> Items, int Total)> GetPagedDtoAsync(CauHoiQuery searchOption);
    }
}

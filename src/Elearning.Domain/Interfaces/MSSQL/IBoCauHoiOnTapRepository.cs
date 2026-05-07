using Elearning.Domain.Entities;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Interfaces.MSSQL
{
    public interface IBoCauHoiOnTapRepository : IRepository<BoCauHoiOnTap>
    {
        Task<(List<BoCauHoiOnTapDto> Items, int Total)> GetPagedDtoAsync(BoCauHoiOnTapQuery searchOption);
    }
}

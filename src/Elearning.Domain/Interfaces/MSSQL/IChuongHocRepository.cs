using Elearning.Domain.Entities;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;

namespace Elearning.Domain.Interfaces.MSSQL
{
    public interface IChuongHocRepository : IRepository<ChuongHoc>
    {
        Task<(List<ChuongHocDto> Items, int Total)> GetPagedDtoAsync(ChuongHocQuery searchOption);
    }
}

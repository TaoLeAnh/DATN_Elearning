using Elearning.Domain.Entities;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;


namespace Elearning.Domain.Interfaces.MSSQL
{
    public interface IKhoaHocRepository : IRepository<KhoaHoc>
    {
        Task<(List<KhoaHocDto> Items, int Total)> GetPagedDtoAsync(KhoaHocQuery searchOption);
    }
}

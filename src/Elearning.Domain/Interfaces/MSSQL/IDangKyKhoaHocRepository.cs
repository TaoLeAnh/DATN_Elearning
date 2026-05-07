using Elearning.Domain.Entities;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Interfaces.MSSQL
{
    public interface IDangKyKhoaHocRepository : IRepository<DangKyKhoaHoc>
    {
        Task<(List<DangKyKhoaHocDto> Items, int Total)> GetPagedDtoAsync(DangKyKhoaHocQuery searchOption);
        Task<List<MyCourseDto>> GetMyCoursesPubAsync(Guid userId);
    }
}

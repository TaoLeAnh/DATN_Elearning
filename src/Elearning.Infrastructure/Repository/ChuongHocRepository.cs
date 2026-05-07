using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;

namespace Elearning.Infrastructure.Repository
{
    public class ChuongHocRepository : Repository<ChuongHoc>, IChuongHocRepository
    {
        private readonly ElearningDbContext _context;

        public ChuongHocRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<ChuongHocDto> Items, int Total)> GetPagedDtoAsync(ChuongHocQuery searchOption)
        {
            int total = 0;

            var baseQuery = FilterData(
                q => q.Include(x => x.KhoaHoc) 
                      .Where(x => !searchOption.isgetBylisID
                                  || searchOption.lstIDGet.Contains(x.Id)),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new ChuongHocDto
            {
                Id = x.Id,
                TenChuong = x.TenChuong,
                KhoaHocId = x.KhoaHocId,
                TenKhoaHoc = x.KhoaHoc != null ? x.KhoaHoc.TenKhoaHoc : string.Empty,
                ThuTu = x.ThuTu,
                Created = x.Created,
                LastModified = x.LastModified
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}
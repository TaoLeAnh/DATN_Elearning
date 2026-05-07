using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;


namespace Elearning.Infrastructure.Repository
{
    public class KhoaHocRepository : Repository<KhoaHoc>, IKhoaHocRepository
    {
        private readonly ElearningDbContext _context;

        public KhoaHocRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<KhoaHocDto> Items, int Total)> GetPagedDtoAsync(KhoaHocQuery searchOption)
        {
            int total = 0;
            var baseQuery = FilterData(
                q => q.Where(x => !searchOption.isgetBylisID
                                  || searchOption.lstIDGet.Contains(x.Id)),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new KhoaHocDto
            {
                Id = x.Id,
                TenKhoaHoc = x.TenKhoaHoc,
                MoTa = x.MoTa,
                MonHoc = x.MonHoc,
                GiangVienId = x.GiangVienId,
                Created = x.Created,
                LastModified = x.LastModified
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

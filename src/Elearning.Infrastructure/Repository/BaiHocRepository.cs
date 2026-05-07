using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;

namespace Elearning.Infrastructure.Repository
{
    public class BaiHocRepository : Repository<BaiHoc>, IBaiHocRepository
    {
        private readonly ElearningDbContext _context;

        public BaiHocRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<BaiHocDto> Items, int Total)> GetPagedDtoAsync(BaiHocQuery searchOption)
        {
            int total = 0;
            var baseQuery = FilterData(
                q => q.Include(x => x.ChuongHoc)
                      .Where(x => !searchOption.isgetBylisID || searchOption.lstIDGet.Contains(x.Id)),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new BaiHocDto
            {
                Id = x.Id,
                TieuDe = x.TieuDe,
                NoiDung = x.NoiDung,
                VideoUrl = x.VideoUrl,
                ThoiLuong = x.ThoiLuong,
                Loai = x.Loai,
                ChuongHocId = x.ChuongHocId,
                TenChuong = x.ChuongHoc.TenChuong,
                ThuTu = x.ThuTu,
                Created = x.Created,
                LastModified = x.LastModified
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

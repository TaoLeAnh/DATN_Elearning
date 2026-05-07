using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Publising.Infrastructure.Persistence.Context;
using Elearning.Publising.Infrastructure.Repositories.Bases;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;

namespace Elearning.Publising.Infrastructure.Repositories
{
    public class KhoaHocRepository : Repository<KhoaHoc>, IKhoaHocRepository
    {
        private readonly AppDbContext _context;

        public KhoaHocRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<KhoaHocDto> Items, int Total)> GetPagedDtoAsync(KhoaHocQuery searchOption)
        {
            int total = 0;

            // Xử lý logic lọc dữ liệu cơ bản
            var baseQuery = FilterData(
                q => q.Where(x => !searchOption.isgetBylisID || searchOption.lstIDGet.Contains(x.Id))
                      // Nếu sau này KhoaHocQuery có truyền CapHoc hoặc MonHoc thì filter thêm ở đây
                       .Where(x => searchOption.MonHoc == null || x.MonHoc == searchOption.MonHoc)
                      .OrderByDescending(x => x.Created),
                searchOption.gridRequest,
                ref total);

            // Tối ưu: Select thẳng sang DTO từ SQL Server, kèm Enum MonHoc
            var dtoQuery = baseQuery.Select(x => new KhoaHocDto
            {
                Id = x.Id,
                TenKhoaHoc = x.TenKhoaHoc,
                MoTa = x.MoTa,
                GiangVienId = x.GiangVienId,
                TenGiangVien = x.GiangVien.Ten,
                SoBaiHoc = x.ChuongHocs.SelectMany(c => c.BaiHocs).Count(),
                HinhAnhUrl = x.HinhAnhUrl,
                MonHoc = x.MonHoc, // Ánh xạ Enum Môn Học
                Created = x.Created,
                LastModified = x.LastModified
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

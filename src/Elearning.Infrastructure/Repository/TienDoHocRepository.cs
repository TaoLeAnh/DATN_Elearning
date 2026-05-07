using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Repository
{
    public class TienDoHocRepository : Repository<TienDoHoc>, ITienDoHocRepository
    {
        private readonly ElearningDbContext _context;

        public TienDoHocRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<TienDoHocDto> Items, int Total)> GetPagedDtoAsync(TienDoHocQuery searchOption)
        {
            int total = 0;
            var baseQuery = FilterData(
                q => q.Include(x => x.NguoiDung).Include(x => x.BaiHoc)
                      .Where(x => !searchOption.isgetBylisID || searchOption.lstIDGet.Contains(x.Id)),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new TienDoHocDto
            {
                Id = x.Id,
                NguoiDungId = x.NguoiDungId,
                TenNguoiDung = x.NguoiDung.Ten,
                BaiHocId = x.BaiHocId,
                TieuDeBaiHoc = x.BaiHoc.TieuDe,
                DaHoanThanh = x.DaHoanThanh,
                ThoiDiemHoanThanh = x.ThoiDiemHoanThanh,
                LastTimePosition = x.LastTimePosition,
                Created = x.Created,
                LastModified = x.LastModified
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

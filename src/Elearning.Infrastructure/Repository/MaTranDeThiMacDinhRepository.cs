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
    public class MaTranDeThiMacDinhRepository : Repository<MaTranDeThiMacDinh>, IMaTranDeThiMacDinhRepository
    {
        private readonly ElearningDbContext _context;

        public MaTranDeThiMacDinhRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<MaTranDeThiMacDinhDto> Items, int Total)> GetPagedDtoAsync(MaTranDeThiMacDinhQuery searchOption)
        {
            int total = 0;
            var baseQuery = FilterData(
                q => q.Include(x => x.ChiTiets) // Kéo theo chi tiết để tính tổng câu
                      .Where(x => !searchOption.isgetBylisID || searchOption.lstIDGet.Contains(x.Id)),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new MaTranDeThiMacDinhDto
            {
                Id = x.Id,
                MonHoc = x.MonHoc,
                TenMaTran = x.TenMaTran,
                IsActive = x.IsActive,
                Created = x.Created,
                LastModified = x.LastModified,
                TongSoCau = x.ChiTiets.Sum(c => c.SoLuong)
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

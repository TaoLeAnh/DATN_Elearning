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
    public class CauHoiRepository : Repository<CauHoi>, ICauHoiRepository
    {
        private readonly ElearningDbContext _context;

        public CauHoiRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<CauHoiDto> Items, int Total)> GetPagedDtoAsync(CauHoiQuery searchOption)
        {
            int total = 0;
            var baseQuery = FilterData(
                q => q.Include(x => x.GiangVien)
                      .Where(x => !searchOption.isgetBylisID || searchOption.lstIDGet.Contains(x.Id)),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new CauHoiDto
            {
                Id = x.Id,
                NoiDung = x.NoiDung,
                HinhAnhUrl = x.HinhAnhUrl,
                LoaiCauHoi = x.LoaiCauHoi,
                MucDo = x.MucDo,
                ChuDe = x.ChuDe,
                GiaiThich = x.GiaiThich,
                GiangVienId = x.GiangVienId,
                TenGiangVien = x.GiangVien.Ten,
                Created = x.Created,
                LastModified = x.LastModified
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Publising.Infrastructure.Persistence.Context;
using Elearning.Publising.Infrastructure.Repositories.Bases;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Portal.Querys;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Infrastructure.Repositories
{

    public class HoSoGiaoVienRepository : Repository<HoSoGiaoVien>, IHoSoGiaoVienRepository
    {
        private readonly AppDbContext _context;

        public HoSoGiaoVienRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<(List<HoSoGiaoVienDto> Items, int Total)> GetPagedDtoAsync(HoSoGiaoVienQuery searchOption)
        {
            throw new NotImplementedException();
        }
        public async Task<List<HoSoGiaoVienDto>> GetDanhSachGiaoVienPubAsync(MonHocEnum? monHoc = null)
        {
            var query = _context.HoSoGiaoViens.AsNoTracking()
                .Include(x => x.NguoiDung)
                .AsQueryable();

            if (monHoc.HasValue)
            {
                query = query.Where(x => x.MonHocChuyenMon == monHoc.Value);
            }

            return await query.Select(x => new HoSoGiaoVienDto
            {
                Id = x.Id,
                NguoiDungId = x.NguoiDungId,
                TenGiaoVien = x.NguoiDung.Ten,
                AnhDaiDienUrl = x.AnhDaiDienUrl,
                MonHocChuyenMon = x.MonHocChuyenMon,
                ThanhTichNoiBat = x.ThanhTichNoiBat,
                PhuongPhapGiangDay = x.PhuongPhapGiangDay
            }).ToListAsync();
        }
    }
}

using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Repository
{
    public class HoSoGiaoVienRepository : Repository<HoSoGiaoVien>, IHoSoGiaoVienRepository
    {
        private readonly ElearningDbContext _context;

        public HoSoGiaoVienRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<HoSoGiaoVienDto>> GetDanhSachGiaoVienPubAsync(MonHocEnum? monHoc = null)
        {
            throw new NotImplementedException();
        }

        public async Task<(List<HoSoGiaoVienDto> Items, int Total)> GetPagedDtoAsync(HoSoGiaoVienQuery searchOption)
        {
            int total = 0;
            var baseQuery = FilterData(
                q => q.Include(x => x.NguoiDung) // Join với bảng NguoiDung để lấy Tên
                      .Where(x => !searchOption.isgetBylisID || searchOption.lstIDGet.Contains(x.Id)),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new HoSoGiaoVienDto
            {
                Id = x.Id,
                NguoiDungId = x.NguoiDungId,
                TenGiaoVien = x.NguoiDung.Ten, // Lấy tên từ bảng người dùng
                AnhDaiDienUrl = x.AnhDaiDienUrl,
                MonHocChuyenMon = x.MonHocChuyenMon,
                ThanhTichNoiBat = x.ThanhTichNoiBat,
                PhuongPhapGiangDay = x.PhuongPhapGiangDay
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

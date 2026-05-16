using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Repository
{
    public class BaiLamRepository : Repository<BaiLam>, IBaiLamRepository
    {
        private readonly ElearningDbContext _context;

        public BaiLamRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<BaiLamDto> Items, int Total)> GetPagedDtoAsync(BaiLamQuery searchOption)
        {
            int total = 0;

            // 1. Build Query lọc dữ liệu
            var baseQuery = FilterData(
                q => q.Include(x => x.NguoiDung)
                      .Include(x => x.LogViPhams)
                      .Include(x => x.KyThi)
                      .Where(x => searchOption.KyThiId == Guid.Empty || x.KyThiId == searchOption.KyThiId)
                      .Where(x => !searchOption.NguoiDungId.HasValue || x.NguoiDungId == searchOption.NguoiDungId.Value)
                      .Where(x => string.IsNullOrEmpty(searchOption.Keyword) ||
                                  x.NguoiDung.Ten.Contains(searchOption.Keyword)),
                searchOption.gridRequest,
                ref total);

            // 2. Select sang DTO
            var dtoQuery = baseQuery.Select(x => new BaiLamDto
            {
                Id = x.Id,
                KyThiId = x.KyThiId,
                TenKyThi = x.KyThi != null ? x.KyThi.TenKyThi : "Không xác định",
                IsKyThiPublic = x.KyThi != null && x.KyThi.IsPublic,
                MonHoc = (x.KyThi != null && x.KyThi.MonHoc.HasValue) ? x.KyThi.MonHoc.ToString() : string.Empty,
                TongSoCau = x.ChiTietBaiLams != null ? x.ChiTietBaiLams.Count() : 0,
                NguoiDungId = x.NguoiDungId,
                TenSinhVien = x.NguoiDung != null ? (x.NguoiDung.Ten ?? "Không có tên") : "Học viên ẩn danh",
                ThoiDiemBatDau = x.ThoiDiemBatDau,
                ThoiDiemNop = x.ThoiDiemNop,
                Diem = x.Diem,
                SoCauDung = x.SoCauDung,
                TrangThai = x.TrangThai,
                Created = x.Created,
                LastModified = x.LastModified,
                TongSoLanViPham = x.LogViPhams != null ? x.LogViPhams.Count() : 0
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

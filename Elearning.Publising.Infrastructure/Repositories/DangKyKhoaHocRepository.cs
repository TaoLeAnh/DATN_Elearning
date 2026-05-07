using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Publising.Infrastructure.Persistence.Context;
using Elearning.Publising.Infrastructure.Repositories.Bases;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Infrastructure.Repositories
{
    public class DangKyKhoaHocRepository : Repository<DangKyKhoaHoc>, IDangKyKhoaHocRepository
    {
        private readonly AppDbContext _context;

        public DangKyKhoaHocRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<(List<DangKyKhoaHocDto> Items, int Total)> GetPagedDtoAsync(DangKyKhoaHocQuery searchOption)
        {
            throw new NotImplementedException();
        }
        public async Task<List<MyCourseDto>> GetMyCoursesPubAsync(Guid userId)
        {
            // Chỉ cần Query thẳng từ DB lên là xong, DTO sẽ lo phần tính toán % Tiến độ
            return await _context.DangKyKhoaHocs
                .AsNoTracking()
                .Where(x => x.NguoiDungId == userId)
                .Select(x => new MyCourseDto
                {
                    Id = x.KhoaHocId,
                    TenKhoaHoc = x.KhoaHoc.TenKhoaHoc,
                    HinhAnhUrl = x.KhoaHoc.HinhAnhUrl,
                    TenGiangVien = x.KhoaHoc.GiangVien != null ? x.KhoaHoc.GiangVien.Ten : "Đang cập nhật",
                    MonHoc = x.KhoaHoc.MonHoc,
                    TongSoBaiHoc = x.KhoaHoc.ChuongHocs.SelectMany(c => c.BaiHocs).Count(),

                    SoBaiDaHoanThanh = _context.TienDoHocs.Count(td =>
                                        td.NguoiDungId == userId &&
                                        td.DaHoanThanh &&
                                        td.BaiHoc.ChuongHoc.KhoaHocId == x.KhoaHocId),

                    BaiHocCuoiCungId = _context.TienDoHocs
                                        .Where(td => td.NguoiDungId == userId &&
                                                     td.BaiHoc.ChuongHoc.KhoaHocId == x.KhoaHocId)
                                        .OrderByDescending(td => td.LastModified)
                                        .Select(td => (Guid?)td.BaiHocId)
                                        .FirstOrDefault()
                })
                .ToListAsync();
        }
    }
}

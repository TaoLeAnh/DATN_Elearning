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
    public class LogViPhamRepository : Repository<LogViPham>, ILogViPhamRepository
    {
        private readonly ElearningDbContext _context;

        public LogViPhamRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<LogViPhamDto> Items, int Total)> GetPagedDtoAsync(LogViPhamQuery searchOption)
        {
            int total = 0;

            // Truy vấn lấy cả Bài Làm, Kỳ thi (hoặc Bộ câu hỏi) và Người dùng
            var baseQuery = FilterData(
                q => q.Include(x => x.BaiLam).ThenInclude(b => b.NguoiDung)
                      .Include(x => x.BaiLam).ThenInclude(b => b.KyThi)
                      .Include(x => x.BaiLam).ThenInclude(b => b.BoCauHoiOnTap)
                      .Where(x => !searchOption.isgetBylisID || searchOption.lstIDGet.Contains(x.Id))
                      .Where(x => !searchOption.BaiLamId.HasValue || x.BaiLamId == searchOption.BaiLamId.Value)
                      .Where(x => !searchOption.NguoiDungId.HasValue || x.BaiLam.NguoiDungId == searchOption.NguoiDungId.Value),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new LogViPhamDto
            {
                Id = x.Id,
                BaiLamId = x.BaiLamId,
                LoaiViPham = x.LoaiViPham,
                ThoiDiemViPham = x.ThoiDiemViPham,
                ChiTiet = x.ChiTiet,

                // Lấy thông tin phụ
                TenNguoiDung = (x.BaiLam != null && x.BaiLam.NguoiDung != null) ? x.BaiLam.NguoiDung.Ten : "Không xác định",

                //MaNguoiDung = (x.BaiLam != null && x.BaiLam.NguoiDung != null) ? x.BaiLam.NguoiDung.MaNhanVien : "N/A",

                TenDeThi = (x.BaiLam != null && x.BaiLam.KyThi != null) ? x.BaiLam.KyThi.TenKyThi
                         : ((x.BaiLam != null && x.BaiLam.BoCauHoiOnTap != null) ? x.BaiLam.BoCauHoiOnTap.TenBo : "Không xác định")
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

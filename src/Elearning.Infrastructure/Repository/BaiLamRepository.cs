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
                      .Include(x => x.KyThi) // BỔ SUNG: Include KyThi để lấy thông tin Public/Môn học
                                             // Không cần Include ChiTietBaiLam vì EF Core có thể tự đếm (Count) thông qua Select
                      .Where(x => x.KyThiId == searchOption.KyThiId)
                      .Where(x => string.IsNullOrEmpty(searchOption.Keyword) ||
                                  x.NguoiDung.Ten.Contains(searchOption.Keyword)), // Nếu có tìm theo Mã SV thì thêm logic Contains vào đây
                searchOption.gridRequest,
                ref total);

            // 2. Select sang DTO
            var dtoQuery = baseQuery.Select(x => new BaiLamDto
            {
                Id = x.Id,
                KyThiId = x.KyThiId,
                TenKyThi = x.KyThi != null ? x.KyThi.TenKyThi : null,

                // --- BỔ SUNG CÁC TRƯỜNG PHÂN LOẠI ĐỀ THI ---
                IsKyThiPublic = x.KyThi != null && x.KyThi.IsPublic,
                MonHoc = (x.KyThi != null && x.KyThi.MonHoc.HasValue) ? x.KyThi.MonHoc.ToString() : null,

                // --- BỔ SUNG TỔNG SỐ CÂU HỎI ---
                TongSoCau = x.ChiTietBaiLams.Count(),

                NguoiDungId = x.NguoiDungId,
                TenSinhVien = x.NguoiDung.Ten ?? "N/A",

                // BỔ SUNG MÃ SINH VIÊN (Thay "UserName" bằng trường thực tế trong bảng NguoiDung của bạn, ví dụ "MaSinhVien")
                //MaSinhVien = x.NguoiDung.Ten ?? "N/A",

                ThoiDiemBatDau = x.ThoiDiemBatDau,
                ThoiDiemNop = x.ThoiDiemNop,
                Diem = x.Diem,
                SoCauDung = x.SoCauDung,
                TrangThai = x.TrangThai,
                Created = x.Created,
                LastModified = x.LastModified,

                TongSoLanViPham = x.LogViPhams.Count()
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

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
    public class DangKyKhoaHocRepository : Repository<DangKyKhoaHoc>, IDangKyKhoaHocRepository
    {
        private readonly ElearningDbContext _context;

        public DangKyKhoaHocRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<MyCourseDto>> GetMyCoursesPubAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<(List<DangKyKhoaHocDto> Items, int Total)> GetPagedDtoAsync(DangKyKhoaHocQuery searchOption)
        {
            int total = 0;
            var baseQuery = FilterData(
                q => q.Include(x => x.NguoiDung).Include(x => x.KhoaHoc)
                      .Where(x => !searchOption.isgetBylisID || searchOption.lstIDGet.Contains(x.Id)),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new DangKyKhoaHocDto
            {
                Id = x.Id,
                NguoiDungId = x.NguoiDungId,
                TenNguoiDung = x.NguoiDung.Ten,
                EmailNguoiDung = x.NguoiDung.Email,
                KhoaHocId = x.KhoaHocId,
                TenKhoaHoc = x.KhoaHoc.TenKhoaHoc,
                NgayDangKy = x.NgayDangKy,
                TrangThai = x.TrangThai,
                TienDo = x.TienDo,
                Created = x.Created,
                LastModified = x.LastModified
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

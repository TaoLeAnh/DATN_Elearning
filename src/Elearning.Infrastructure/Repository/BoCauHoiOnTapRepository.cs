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
    public class BoCauHoiOnTapRepository : Repository<BoCauHoiOnTap>, IBoCauHoiOnTapRepository
    {
        private readonly ElearningDbContext _context;

        public BoCauHoiOnTapRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<BoCauHoiOnTapDto> Items, int Total)> GetPagedDtoAsync(BoCauHoiOnTapQuery searchOption)
        {
            int total = 0;
            var baseQuery = FilterData(
                q => q.Include(x => x.GiangVien)
                      .Include(x => x.KhoaHoc)    // <-- Thêm dòng này
                      .Include(x => x.ChuongHoc)
                      .Include(x => x.BaiHoc)
                      .Where(x => !searchOption.isgetBylisID || searchOption.lstIDGet.Contains(x.Id)),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new BoCauHoiOnTapDto
            {
                Id = x.Id,
                TenBo = x.TenBo,
                MoTa = x.MoTa,
                LoaiBoCauHoi = x.LoaiBoCauHoi,

                KhoaHocId = x.KhoaHocId,
                TenKhoaHoc = x.KhoaHoc != null ? x.KhoaHoc.TenKhoaHoc : null, // <-- Thêm dòng này

                ChuongHocId = x.ChuongHocId,
                TenChuongHoc = x.ChuongHoc != null ? x.ChuongHoc.TenChuong : null,

                BaiHocId = x.BaiHocId,
                TenBaiHoc = x.BaiHoc != null ? x.BaiHoc.TieuDe : null,

                GiangVienId = x.GiangVienId,
                TenGiangVien = x.GiangVien != null ? x.GiangVien.Ten : null, // Thêm != null cho an toàn
                SoLuongCauHoi = x.ChiTietBoCauHois.Count(),
                Created = x.Created,
                LastModified = x.LastModified
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

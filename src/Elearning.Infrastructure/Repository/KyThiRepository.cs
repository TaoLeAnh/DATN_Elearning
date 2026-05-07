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
    public class KyThiRepository : Repository<KyThi>, IKyThiRepository
    {
        private readonly ElearningDbContext _context;

        public KyThiRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<KyThiDto> Items, int Total)> GetPagedDtoAsync(KyThiQuery searchOption)
        {
            int total = 0;

            var baseQuery = FilterData(
                q => q.Include(x => x.KhoaHoc)
                        .Include(x => x.CauHoiKyThis)
                        .Include(x => x.BaiLams)
                      .Where(x => !searchOption.isgetBylisID || searchOption.lstIDGet.Contains(x.Id))
                      .Where(x => x.ModerationStatus != Elearning.Shared.Commons.Model.SQL.ModerationStatus.Cancelled)
                      .Where(x => x.LoaiDeThi != Shared.Contracts.Portal.Enums.EnumLoaiDeThi.DeThiNgauNhien),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new KyThiDto
            {
                Id = x.Id,
                TenKyThi = x.TenKyThi,

                // Dùng dấu ? và ?? để tránh lỗi Null
                KhoaHocId = x.KhoaHocId ?? Guid.Empty,
                TenKhoaHoc = x.KhoaHoc != null ? x.KhoaHoc.TenKhoaHoc : null,

                // Nếu bạn đã đổi KyThiDto.ThoiGianBatDau thành Nullable (DateTime?) thì bỏ .Value đi
                // Nếu trong DTO vẫn là DateTime bắt buộc, thì gán một ngày mặc định
                ThoiGianBatDau = x.ThoiGianBatDau, // Giả định trong DTO bạn đã đổi thành DateTime?
                ThoiGianKetThuc = x.ThoiGianKetThuc, // Giả định trong DTO bạn đã đổi thành DateTime?

                ThoiLuongPhut = x.ThoiLuongPhut,
                Created = x.Created,
                LastModified = x.LastModified,
                SoLuongCauHoi = x.CauHoiKyThis.Count(),
                SoLuongBaiLam = x.BaiLams.Count(),

                // Map thêm các trường hiển thị công khai (nếu cần)
                IsPublic = x.IsPublic,
                LoaiDeThi = x.LoaiDeThi
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

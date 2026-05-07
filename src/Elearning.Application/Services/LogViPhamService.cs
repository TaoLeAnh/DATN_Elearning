using Elearning.Application.Interfaces;
using Elearning.Domain.Interfaces;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Services
{
    public class LogViPhamService : ILogViPhamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogViPhamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DataTableJson> GetPaged(LogViPhamQuery searchOption)
        {
            var (items, total) = await _unitOfWork.LogViPhamRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        // Logic GetById nếu bạn cần View chi tiết 1 log riêng
        public async Task<LogViPhamDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.LogViPhamRepository.FindAsync(
                x => x.Id == id,
                includes: new[] { "BaiLam.NguoiDung", "BaiLam.KyThi", "BaiLam.BoCauHoiOnTap" }
            );

            if (entity == null) return null;

            return new LogViPhamDto
            {
                Id = entity.Id,
                LoaiViPham = entity.LoaiViPham,
                ThoiDiemViPham = entity.ThoiDiemViPham,
                ChiTiet = entity.ChiTiet,
                TenNguoiDung = entity.BaiLam?.NguoiDung?.Ten,
                TenDeThi = entity.BaiLam?.KyThi?.TenKyThi ?? entity.BaiLam?.BoCauHoiOnTap?.TenBo
            };
        }
    }
}

using Elearning.Domain.Interfaces;
using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Services
{
    public class HoSoGiaoVienService : IHoSoGiaoVienService
    {
        private readonly IUnitOfWorkPublising _unitOfWork;

        public HoSoGiaoVienService(IUnitOfWorkPublising unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<HoSoGiaoVienDto>> GetDanhSachGiaoVienAsync(MonHocEnum? monHoc = null)
        {
            return await _unitOfWork.HoSoGiaoVienRepository.GetDanhSachGiaoVienPubAsync(monHoc);
        }

        public async Task<HoSoGiaoVienDto?> GetChiTietGiaoVienAsync(Guid id)
        {
            var entity = await _unitOfWork.HoSoGiaoVienRepository.FindAsync(
                x => x.Id == id,
                includes: new[] { "NguoiDung" }
            );

            if (entity == null) return null;

            return new HoSoGiaoVienDto
            {
                Id = entity.Id,
                NguoiDungId = entity.NguoiDungId,
                TenGiaoVien = entity.NguoiDung?.Ten,
                AnhDaiDienUrl = entity.AnhDaiDienUrl,
                MonHocChuyenMon = entity.MonHocChuyenMon,
                ThanhTichNoiBat = entity.ThanhTichNoiBat,
                PhuongPhapGiangDay = entity.PhuongPhapGiangDay
            };
        }
    }
}

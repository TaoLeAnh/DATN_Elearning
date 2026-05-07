using Elearning.Domain.Entities;
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
    public class DangKyKhoaHocService : IDangKyKhoaHocService
    {
        private readonly IUnitOfWorkPublising _unitOfWork;

        public DangKyKhoaHocService(IUnitOfWorkPublising unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MyCourseDto>> GetMyCoursesAsync(Guid userId)
        {
            return await _unitOfWork.DangKyKhoaHocRepository.GetMyCoursesPubAsync(userId);
        }
        public async Task<string> DangKyKhoaHocMoiAsync(Guid userId, Guid khoaHocId)
        {
            var isExist = await _unitOfWork.DangKyKhoaHocRepository.FindAsync(x => x.NguoiDungId == userId && x.KhoaHocId == khoaHocId);

            if (isExist != null)
            {
                return "BẠN_ĐÃ_ĐĂNG_KÝ";
            }
            var dangKyMoi = new DangKyKhoaHoc
            {
                NguoiDungId = userId,
                KhoaHocId = khoaHocId,
                NgayDangKy = DateTime.Now,
                TienDo = 0,
                TrangThai = (EnumTrangThaiDangKy)1
            };

            await _unitOfWork.DangKyKhoaHocRepository.AddAsync(dangKyMoi);
            await _unitOfWork.CompleteAsync();

            return "THÀNH_CÔNG";
        }
        public async Task<int> CountTatCaHocVienAsync()
        {
            var count = await _unitOfWork.DangKyKhoaHocRepository
                .GetTableNoTracking()
                .Select(x => x.NguoiDungId)
                .Distinct()
                .CountAsync();

            return count;
        }
    }
}

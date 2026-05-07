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
    public class BaiLamService : IBaiLamService
    {
        private readonly IUnitOfWorkPublising _unitOfWork;

        public BaiLamService(IUnitOfWorkPublising unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<QuizHistoryDto>> GetQuizHistoryAsync(Guid quizId, Guid userId)
        {
            // ĐÃ SỬA LỖI: Sử dụng EnumTrangThaiBaiLam.DaNop thay vì (EnumTrangThaiBaiLam)1
            var history = await _unitOfWork.BaiLamRepository.GetTableNoTracking()
                .Where(x => x.BoCauHoiOnTapId == quizId
                         && x.NguoiDungId == userId
                         && x.TrangThai == EnumTrangThaiBaiLam.DaNop)
                .OrderByDescending(x => x.ThoiDiemNop) // Mới nhất lên đầu
                .Select(x => new QuizHistoryDto
                {
                    Id = x.Id,
                    Diem = x.Diem,
                    SoCauDung = x.SoCauDung,
                    ThoiDiemBatDau = x.ThoiDiemBatDau,
                    ThoiDiemNop = x.ThoiDiemNop
                })
                .ToListAsync();

            return history;
        }
    }
}

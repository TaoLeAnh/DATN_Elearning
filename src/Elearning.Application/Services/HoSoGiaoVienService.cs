using Elearning.Application.Interfaces;
using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Services
{
    public class HoSoGiaoVienService : IHoSoGiaoVienService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _RequestContext;

        public HoSoGiaoVienService(IUnitOfWork unitOfWork, IRequestContext requestContext)
        {
            _unitOfWork = unitOfWork;
            _RequestContext = requestContext;
        }

        public async Task<DataTableJson> GetPaged(HoSoGiaoVienQuery searchOption)
        {
            var (items, total) = await _unitOfWork.HoSoGiaoVienRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<HoSoGiaoVienDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.HoSoGiaoVienRepository.FindAsync(
                x => x.Id == id,
                includes: new[] { "NguoiDung" } // Join để lấy tên
            );

            if (entity == null) throw new ArgumentException("Không tìm thấy hồ sơ giáo viên.");

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

        public async Task<Guid> CreateAsync(HoSoGiaoVienForm form)
        {
            // Kiểm tra xem giáo viên này đã có hồ sơ chưa (Quan hệ 1-1)
            var existed = await _unitOfWork.HoSoGiaoVienRepository.FindAsync(x => x.NguoiDungId == form.NguoiDungId);
            if (existed != null) throw new ArgumentException("Người dùng này đã có hồ sơ giáo viên.");

            var entity = new HoSoGiaoVien
            {
                NguoiDungId = form.NguoiDungId,
                AnhDaiDienUrl = form.AnhDaiDienUrl,
                MonHocChuyenMon = form.MonHocChuyenMon,
                ThanhTichNoiBat = form.ThanhTichNoiBat,
                PhuongPhapGiangDay = form.PhuongPhapGiangDay
            };

            await _unitOfWork.HoSoGiaoVienRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, HoSoGiaoVienForm item)
        {
            var itemUpdate = await _unitOfWork.HoSoGiaoVienRepository.GetByIdAsync(id);
            if (itemUpdate == null) return false;

            itemUpdate.AnhDaiDienUrl = item.AnhDaiDienUrl;
            itemUpdate.MonHocChuyenMon = item.MonHocChuyenMon;
            itemUpdate.ThanhTichNoiBat = item.ThanhTichNoiBat;
            itemUpdate.PhuongPhapGiangDay = item.PhuongPhapGiangDay;
            // Không cho phép update NguoiDungId vì đây là khóa liên kết 1-1

            _unitOfWork.HoSoGiaoVienRepository.Update(itemUpdate);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var itemDelete = await _unitOfWork.HoSoGiaoVienRepository.GetByIdAsync(id);
            if (itemDelete == null) return false;

            // Xóa cứng theo đúng logic của BaiHocService mẫu
            _unitOfWork.HoSoGiaoVienRepository.Delete(itemDelete);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }
    }
}

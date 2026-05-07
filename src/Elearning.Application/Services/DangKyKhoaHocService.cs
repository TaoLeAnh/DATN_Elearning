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
    public class DangKyKhoaHocService : IDangKyKhoaHocService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _RequestContext;

        public DangKyKhoaHocService(IUnitOfWork unitOfWork, IRequestContext requestContext)
        {
            _unitOfWork = unitOfWork;
            _RequestContext = requestContext;
        }

        public async Task<DataTableJson> GetPaged(DangKyKhoaHocQuery searchOption)
        {
            var (items, total) = await _unitOfWork.DangKyKhoaHocRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<DangKyKhoaHocDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.DangKyKhoaHocRepository.FindAsync(
                x => x.Id == id,
                includes: new[] { "NguoiDung", "KhoaHoc" }
            );

            if (entity == null) throw new ArgumentException("Không tìm thấy bản ghi đăng ký.");

            return new DangKyKhoaHocDto
            {
                Id = entity.Id,
                NguoiDungId = entity.NguoiDungId,
                TenNguoiDung = entity.NguoiDung?.Ten,
                EmailNguoiDung = entity.NguoiDung?.Email,
                KhoaHocId = entity.KhoaHocId,
                TenKhoaHoc = entity.KhoaHoc?.TenKhoaHoc,
                NgayDangKy = entity.NgayDangKy,
                TrangThai = entity.TrangThai,
                TienDo = entity.TienDo,
                Created = entity.Created,
                LastModified = entity.LastModified
            };
        }

        public async Task<Guid> CreateAsync(DangKyKhoaHocForm form)
        {
            // Kiểm tra trùng lặp trước khi thêm
            bool isExist = await _unitOfWork.DangKyKhoaHocRepository.IsExistAsync(
                x => x.NguoiDungId == form.NguoiDungId && x.KhoaHocId == form.KhoaHocId);

            if (isExist) throw new Exception("Người dùng đã đăng ký khóa học này rồi.");

            var entity = new DangKyKhoaHoc
            {
                NguoiDungId = form.NguoiDungId,
                KhoaHocId = form.KhoaHocId,
                TrangThai = form.TrangThai,
                TienDo = form.TienDo,
                NgayDangKy = DateTime.Now
            };

            await _unitOfWork.DangKyKhoaHocRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, DangKyKhoaHocForm item)
        {
            var itemUpdate = await _unitOfWork.DangKyKhoaHocRepository.GetByIdAsync(id);
            if (itemUpdate == null) return false;

            // Không cho phép update NguoiDungId và KhoaHocId để tránh phá vỡ Unique Index
            itemUpdate.TrangThai = item.TrangThai;
            itemUpdate.TienDo = item.TienDo;

            _unitOfWork.DangKyKhoaHocRepository.Update(itemUpdate);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var itemDelete = await _unitOfWork.DangKyKhoaHocRepository.GetByIdAsync(id);
            if (itemDelete == null) return false;

            _unitOfWork.DangKyKhoaHocRepository.Delete(itemDelete);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }
    }
}

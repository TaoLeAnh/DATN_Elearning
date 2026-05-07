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
    public class TienDoHocService : ITienDoHocService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _RequestContext;

        public TienDoHocService(IUnitOfWork unitOfWork, IRequestContext requestContext)
        {
            _unitOfWork = unitOfWork;
            _RequestContext = requestContext;
        }

        public async Task<DataTableJson> GetPaged(TienDoHocQuery searchOption)
        {
            var (items, total) = await _unitOfWork.TienDoHocRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<TienDoHocDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.TienDoHocRepository.FindAsync(
                x => x.Id == id,
                includes: new[] { "NguoiDung", "BaiHoc" }
            );

            if (entity == null) throw new ArgumentException("Không tìm thấy tiến độ học.");

            return new TienDoHocDto
            {
                Id = entity.Id,
                NguoiDungId = entity.NguoiDungId,
                TenNguoiDung = entity.NguoiDung?.Ten,
                BaiHocId = entity.BaiHocId,
                TieuDeBaiHoc = entity.BaiHoc?.TieuDe,
                DaHoanThanh = entity.DaHoanThanh,
                ThoiDiemHoanThanh = entity.ThoiDiemHoanThanh,
                LastTimePosition = entity.LastTimePosition,
                Created = entity.Created,
                LastModified = entity.LastModified
            };
        }

        public async Task<Guid> CreateAsync(TienDoHocForm form)
        {
            // Kiểm tra trùng lặp
            bool isExist = await _unitOfWork.TienDoHocRepository.IsExistAsync(
                x => x.NguoiDungId == form.NguoiDungId && x.BaiHocId == form.BaiHocId);

            if (isExist) throw new Exception("Tiến độ của bài học này đã tồn tại.");

            var entity = new TienDoHoc
            {
                NguoiDungId = form.NguoiDungId,
                BaiHocId = form.BaiHocId,
                DaHoanThanh = form.DaHoanThanh,
                ThoiDiemHoanThanh = form.DaHoanThanh ? DateTime.Now : null,
                LastTimePosition = form.LastTimePosition
            };

            await _unitOfWork.TienDoHocRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, TienDoHocForm item)
        {
            var itemUpdate = await _unitOfWork.TienDoHocRepository.GetByIdAsync(id);
            if (itemUpdate == null) return false;

            itemUpdate.DaHoanThanh = item.DaHoanThanh;
            if (item.DaHoanThanh && itemUpdate.ThoiDiemHoanThanh == null)
            {
                itemUpdate.ThoiDiemHoanThanh = DateTime.Now;
            }
            itemUpdate.LastTimePosition = item.LastTimePosition;

            _unitOfWork.TienDoHocRepository.Update(itemUpdate);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var itemDelete = await _unitOfWork.TienDoHocRepository.GetByIdAsync(id);
            if (itemDelete == null) return false;

            _unitOfWork.TienDoHocRepository.Delete(itemDelete);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }
    }
}

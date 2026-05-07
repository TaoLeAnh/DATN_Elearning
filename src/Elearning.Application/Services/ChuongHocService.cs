using Elearning.Application.Interfaces;
using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;

namespace Elearning.Application.Services
{
    public class ChuongHocService : IChuongHocService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _RequestContext;

        public ChuongHocService(IUnitOfWork unitOfWork, IRequestContext requestContext)
        {
            _unitOfWork = unitOfWork;
            _RequestContext = requestContext;
        }

        public async Task<DataTableJson> GetPaged(ChuongHocQuery searchOption)
        {
            var (items, total) = await _unitOfWork.ChuongHocRepository.GetPagedDtoAsync(searchOption);

            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<ChuongHocDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.ChuongHocRepository.FindAsync(
                x => x.Id == id,
                includes: new[] { "KhoaHoc" }
            );

            if (entity == null)
            {
                throw new ArgumentException("Không tìm thấy bản ghi chương học.");
            }

            return new ChuongHocDto
            {
                Id = entity.Id,
                TenChuong = entity.TenChuong,
                KhoaHocId = entity.KhoaHocId,
                TenKhoaHoc = entity.KhoaHoc?.TenKhoaHoc,
                ThuTu = entity.ThuTu,
                Created = entity.Created,
                LastModified = entity.LastModified
            };
        }

        public async Task<Guid> CreateAsync(ChuongHocForm form)
        {
            var entity = new ChuongHoc
            {
                TenChuong = form.TenChuong,
                KhoaHocId = form.KhoaHocId,
                ThuTu = form.ThuTu
            };

            await _unitOfWork.ChuongHocRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, ChuongHocForm item)
        {
            var itemUpdate = await _unitOfWork.ChuongHocRepository.GetByIdAsync(id);
            if (itemUpdate == null)
                throw new KeyNotFoundException($"Không tìm thấy chương học với Id = {id}");

            itemUpdate.TenChuong = item.TenChuong;
            itemUpdate.KhoaHocId = item.KhoaHocId;
            itemUpdate.ThuTu = item.ThuTu;

            _unitOfWork.ChuongHocRepository.Update(itemUpdate);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);

            return true;
        }

        public async Task<bool> DeleteAsync(Guid Id)
        {
            var itemDelete = await _unitOfWork.ChuongHocRepository.GetByIdAsync(Id);

            if (itemDelete == null)
                throw new KeyNotFoundException($"Không tìm thấy chương học để xóa.");

            _unitOfWork.ChuongHocRepository.Delete(itemDelete);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);

            return true;
        }
    }
}

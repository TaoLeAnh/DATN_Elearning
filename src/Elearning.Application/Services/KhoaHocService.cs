using Elearning.Application.Interfaces;
using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;

namespace Elearning.Application.Services
{
    public class KhoaHocService : IKhoaHocService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _RequestContext;
        public KhoaHocService(IUnitOfWork unitOfWork, IRequestContext requestContext)
        {
            _unitOfWork = unitOfWork;
            _RequestContext = requestContext;
        }

        public async Task<DataTableJson> GetPaged(KhoaHocQuery searchOption)
        {
            var (items, total) = await _unitOfWork.KhoaHocRepository.GetPagedDtoAsync(searchOption);

            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<KhoaHocDto> GetByIdAsync(Guid id)
        {
            KhoaHoc entity = await _unitOfWork.KhoaHocRepository.FindAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new ArgumentException("Không tìm thấy bản ghi khóa học.");
            }

            return new KhoaHocDto
            {
                Id = entity.Id,
                TenKhoaHoc = entity.TenKhoaHoc,
                MoTa = entity.MoTa,
                GiangVienId = entity.GiangVienId,
                MonHoc = entity.MonHoc,
                HinhAnhUrl = entity.HinhAnhUrl,
                GiaGoc = entity.GiaGoc,
                GiaBan = entity.GiaBan,
                Created = entity.Created,
                LastModified = entity.LastModified
            };
        }

        public async Task<Guid> CreateAsync(KhoaHocForm form)
        {
            var entity = new KhoaHoc
            {
                TenKhoaHoc = form.TenKhoaHoc,
                MoTa = form.MoTa,
                GiangVienId = form.GiangVienId,
                MonHoc = form.MonHoc,
                HinhAnhUrl = form.HinhAnhUrl,
                GiaGoc = form.GiaGoc,
                GiaBan = form.GiaBan
            };

            await _unitOfWork.KhoaHocRepository.AddAsync(entity);

            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);

            return entity.Id;
        }
        public async Task<bool> UpdateAsync(Guid id, KhoaHocForm item)
        {
            var itemUpdate = await _unitOfWork.KhoaHocRepository.GetByIdAsync(id);
            if (itemUpdate == null)
                throw new KeyNotFoundException($"Không tìm thấy khoa hoc với Id = {id}");

            itemUpdate.TenKhoaHoc = item.TenKhoaHoc;
            itemUpdate.MoTa = item.MoTa;
            itemUpdate.GiangVienId = item.GiangVienId;
            itemUpdate.MonHoc = item.MonHoc;
            itemUpdate.HinhAnhUrl = item.HinhAnhUrl;
            itemUpdate.GiaGoc = item.GiaGoc;
            itemUpdate.GiaBan = item.GiaBan;

            _unitOfWork.KhoaHocRepository.Update(itemUpdate);

            // Thay vì SaveChanges của Repository, hãy dùng UnitOfWork để thống nhất
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);

            return true;
        }

        public async Task<bool> DeleteAsync(Guid Id)
        {
            var itemDelete = await _unitOfWork.KhoaHocRepository.GetByIdAsync(Id);
            _unitOfWork.KhoaHocRepository.Delete(itemDelete);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);

            return true;
        }

        public async Task<List<KhoaHocDto>> GetAllAsync()
        {
            // Lấy toàn bộ khóa học, sắp xếp theo tên A-Z
            var list = await _unitOfWork.KhoaHocRepository.GetTableNoTracking()
                .OrderBy(x => x.TenKhoaHoc)
                .Select(x => new KhoaHocDto
                {
                    Id = x.Id,
                    TenKhoaHoc = x.TenKhoaHoc
                })
                .ToListAsync();

            return list;
        }
    }
}

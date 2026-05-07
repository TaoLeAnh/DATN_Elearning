using Elearning.Application.Interfaces;
using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Infrastructure.Security;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;

namespace Elearning.Application.Services
{
    public class NguoiDungService : INguoiDungService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext; // Tái sử dụng RequestContext của công ty

        public NguoiDungService(IUnitOfWork unitOfWork, IRequestContext requestContext)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
        }

        public async Task<DataTableJson> GetPaged(NguoiDungQuery searchOption)
        {
            var (items, total) = await _unitOfWork.NguoiDungRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<NguoiDungDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.NguoiDungRepository.FindAsync(x => x.Id == id);

            if (entity == null)
                throw new ArgumentException("Không tìm thấy người dùng.");

            return new NguoiDungDto
            {
                Id = entity.Id,
                Ten = entity.Ten,
                Email = entity.Email,
                VaiTro = entity.VaiTro,
                Created = entity.Created,
                LastModified = entity.LastModified
            };
        }

        public async Task<Guid> CreateAsync(NguoiDungForm form)
        {
            var hasher = new BCryptPasswordHasher();
            var entity = new NguoiDung
            {
                Ten = form.Ten,
                Email = form.Email,
                VaiTro = form.VaiTro,
                MatKhau = hasher.Hash(form.MatKhau)
            };

            await _unitOfWork.NguoiDungRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(new Guid("99999999-9999-9999-9999-999999999999"));

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, NguoiDungForm item)
        {
            var entityUpdate = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);

            if (entityUpdate == null)
                throw new KeyNotFoundException($"Không tìm thấy người dùng với Id = {id}");

            entityUpdate.Ten = item.Ten;
            entityUpdate.Email = item.Email;
            entityUpdate.VaiTro = item.VaiTro;

            if (!string.IsNullOrWhiteSpace(item.MatKhau))
            {
                var hasher = new BCryptPasswordHasher();
                entityUpdate.MatKhau = hasher.Hash(item.MatKhau);
            }

            _unitOfWork.NguoiDungRepository.Update(entityUpdate);

            _unitOfWork.NguoiDungRepository.SaveChanges(_requestContext.CurrentIdUser);

            return true;
        }

        public async Task<bool> DeleteAsync(Guid Id)
        {
            var entityDelete = await _unitOfWork.NguoiDungRepository.GetByIdAsync(Id);

            if (entityDelete == null)
                throw new KeyNotFoundException($"Không tìm thấy người dùng để xóa.");

            _unitOfWork.NguoiDungRepository.Delete(entityDelete);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}

using Elearning.Application.Interfaces;
using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Services
{
    public class MaTranDeThiMacDinhService : IMaTranDeThiMacDinhService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _RequestContext;

        public MaTranDeThiMacDinhService(IUnitOfWork unitOfWork, IRequestContext requestContext)
        {
            _unitOfWork = unitOfWork;
            _RequestContext = requestContext;
        }

        public async Task<DataTableJson> GetPaged(MaTranDeThiMacDinhQuery searchOption)
        {
            var (items, total) = await _unitOfWork.MaTranDeThiMacDinhRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<MaTranDeThiMacDinhDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.MaTranDeThiMacDinhRepository.FindAsync(
                x => x.Id == id,
                includes: new[] { "ChiTiets" }
            );

            if (entity == null) throw new ArgumentException("Không tìm thấy ma trận.");

            return new MaTranDeThiMacDinhDto
            {
                Id = entity.Id,
                MonHoc = entity.MonHoc,
                TenMaTran = entity.TenMaTran,
                IsActive = entity.IsActive,
                ChiTiets = entity.ChiTiets.Select(c => new ChiTietMaTranMacDinhDto
                {
                    PhanThi = c.PhanThi,
                    LoaiCauHoi = c.LoaiCauHoi,
                    MucDo = c.MucDo,
                    ChuDe = c.ChuDe,
                    SoLuong = c.SoLuong
                }).ToList()
            };
        }

        public async Task<Guid> CreateAsync(MaTranDeThiMacDinhForm form)
        {
            // LOGIC BẢO VỆ: Nếu set Active, phải tắt các ma trận khác cùng môn học
            if (form.IsActive)
            {
                await DeactivateOtherMatricesAsync(form.MonHoc, Guid.Empty);
            }

            var entity = new MaTranDeThiMacDinh
            {
                TenMaTran = form.TenMaTran,
                MonHoc = form.MonHoc,
                IsActive = form.IsActive,
                ChiTiets = form.ChiTiets.Select(c => new ChiTietMaTranMacDinh
                {
                    PhanThi = c.PhanThi,
                    LoaiCauHoi = c.LoaiCauHoi,
                    MucDo = c.MucDo,
                    ChuDe = c.ChuDe,
                    SoLuong = c.SoLuong
                }).ToList()
            };

            await _unitOfWork.MaTranDeThiMacDinhRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, MaTranDeThiMacDinhForm item)
        {
            // Load entity và các chi tiết hiện tại
            var itemUpdate = await _unitOfWork.MaTranDeThiMacDinhRepository.GetTableAsTracking()
                .Include(x => x.ChiTiets)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (itemUpdate == null) return false;

            if (item.IsActive)
            {
                await DeactivateOtherMatricesAsync(item.MonHoc, id);
            }

            itemUpdate.TenMaTran = item.TenMaTran;
            itemUpdate.MonHoc = item.MonHoc;
            itemUpdate.IsActive = item.IsActive;

            // Xóa chi tiết cũ và thêm chi tiết mới để đảm bảo đồng bộ
            _unitOfWork.ChiTietMaTranMacDinhRepository.DeleteRange(itemUpdate.ChiTiets.ToList());

            var newChiTiets = item.ChiTiets.Select(c => new ChiTietMaTranMacDinh
            {
                MaTranDeThiMacDinhId = id,
                PhanThi = c.PhanThi,
                LoaiCauHoi = c.LoaiCauHoi,
                MucDo = c.MucDo,
                ChuDe = c.ChuDe,
                SoLuong = c.SoLuong
            }).ToList();

            await _unitOfWork.ChiTietMaTranMacDinhRepository.AddRangeAsync(newChiTiets);
            _unitOfWork.MaTranDeThiMacDinhRepository.Update(itemUpdate);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var itemDelete = await _unitOfWork.MaTranDeThiMacDinhRepository.GetTableAsTracking()
                .Include(x => x.ChiTiets)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (itemDelete == null) return false;
            if (itemDelete.ChiTiets != null && itemDelete.ChiTiets.Any())
            {
                _unitOfWork.ChiTietMaTranMacDinhRepository.DeleteRange(itemDelete.ChiTiets.ToList());
            }

            _unitOfWork.MaTranDeThiMacDinhRepository.Delete(itemDelete);

            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }

        private async Task DeactivateOtherMatricesAsync(Elearning.Shared.Contracts.Portal.Enums.MonHocEnum monHoc, Guid excludeId)
        {
            var others = await _unitOfWork.MaTranDeThiMacDinhRepository.GetTableAsTracking()
                .Where(x => x.MonHoc == monHoc && x.IsActive && x.Id != excludeId)
                .ToListAsync();

            foreach (var m in others)
            {
                m.IsActive = false;
                _unitOfWork.MaTranDeThiMacDinhRepository.Update(m);
            }
        }
        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var entity = await _unitOfWork.MaTranDeThiMacDinhRepository.GetByIdAsync(id);
            if (entity == null) return false;

            // Nếu đang trạng thái TẮT mà muốn BẬT lên (Active = true)
            if (!entity.IsActive)
            {
                // Gọi hàm phụ trợ đã có sẵn của bạn để tắt hết các thằng khác cùng môn
                await DeactivateOtherMatricesAsync(entity.MonHoc, id);
                entity.IsActive = true;
            }
            else
            {
                // Nếu đang BẬT thì chỉ đơn giản là TẮT đi
                entity.IsActive = false;
            }

            _unitOfWork.MaTranDeThiMacDinhRepository.Update(entity);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }
        public async Task<List<MaTranDeThiMacDinhDto>> GetActiveByKyThiIdAsync(Guid kyThiId)
        {
            // 1. Lấy thông tin Kỳ thi
            var kyThi = await _unitOfWork.KyThiRepository.GetByIdAsync(kyThiId);
            if (kyThi == null)
            {
                return new List<MaTranDeThiMacDinhDto>();
            }

            // 2. TÌM MÔN HỌC ĐÍCH CỦA KỲ THI
            // Ưu tiên 1: Lấy môn học được gán trực tiếp trên Kỳ thi
            Elearning.Shared.Contracts.Portal.Enums.MonHocEnum? targetMonHoc = kyThi.MonHoc;

            // Ưu tiên 2: Nếu Kỳ thi không gán Môn trực tiếp nhưng có Khóa học, thì lấy Môn từ Khóa học
            if (targetMonHoc == null && kyThi.KhoaHocId.HasValue)
            {
                var khoaHoc = await _unitOfWork.KhoaHocRepository.GetByIdAsync(kyThi.KhoaHocId.Value);
                if (khoaHoc != null)
                {
                    targetMonHoc = khoaHoc.MonHoc;
                }
            }

            // 3. XÂY DỰNG TRUY VẤN TÌM MA TRẬN
            var query = _unitOfWork.MaTranDeThiMacDinhRepository
                .GetTableNoTracking()
                .Include(x => x.ChiTiets) // Kéo theo chi tiết để tính tổng câu hỏi
                .Where(x => x.IsActive);  // Chỉ lấy ma trận đang Bật

            // Nếu xác định được Môn học (từ Kỳ thi hoặc Khóa học), thì bắt buộc lọc theo Môn đó.
            // (Lưu ý: Nếu Kỳ thi trơ trọi, ko môn, ko khóa học, query sẽ thả cửa lấy TẤT CẢ ma trận cho user tự chọn)
            if (targetMonHoc.HasValue)
            {
                query = query.Where(x => x.MonHoc == targetMonHoc.Value);
            }

            // 4. THỰC THI TRUY VẤN VÀ ĐỔ DỮ LIỆU RA DTO
            var dsMaTran = await query
                .Select(x => new MaTranDeThiMacDinhDto
                {
                    Id = x.Id,
                    TenMaTran = x.TenMaTran,
                    MonHoc = x.MonHoc,
                    // An toàn tuyệt đối không bao giờ dính lỗi NullReferenceException
                    TongSoCau = x.ChiTiets != null ? x.ChiTiets.Sum(c => c.SoLuong) : 0,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return dsMaTran;
        }
    }
}

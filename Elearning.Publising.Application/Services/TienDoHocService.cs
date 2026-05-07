using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Publising.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Elearning.Publising.Application.Services
{
    public class TienDoHocService : ITienDoHocService
    {
        private readonly IUnitOfWorkPublising _unitOfWork;

        public TienDoHocService(IUnitOfWorkPublising unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Lấy danh sách ID các bài học mà User này đã học xong trong Khóa học cụ thể
        public async Task<List<Guid>> GetCompletedLessonIdsAsync(Guid courseId, Guid userId)
        {
            // Lấy tất cả bài học thuộc khóa học này
            var baiHocIds = await _unitOfWork.ChuongHocRepository.GetTableNoTracking()
                .Where(c => c.KhoaHocId == courseId)
                .SelectMany(c => c.BaiHocs.Select(b => b.Id))
                .ToListAsync();

            // Tìm trong bảng Tiến độ xem bài nào trùng khớp và đã hoàn thành
            var completed = await _unitOfWork.TienDoHocRepository.GetTableNoTracking()
                .Where(t => t.NguoiDungId == userId && baiHocIds.Contains(t.BaiHocId) && t.DaHoanThanh)
                .Select(t => t.BaiHocId)
                .ToListAsync();

            return completed;
        }

        // Đánh dấu hoàn thành bài học
        public async Task<bool> MarkLessonCompleteAsync(Guid userId, Guid baiHocId)
        {
            // Tìm xem trước đây có bản ghi nào chưa (VD: đang xem dở)
            var tienDo = await _unitOfWork.TienDoHocRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(t => t.NguoiDungId == userId && t.BaiHocId == baiHocId);

            if (tienDo != null)
            {
                tienDo.DaHoanThanh = true;
                tienDo.ThoiDiemHoanThanh = DateTime.Now;
                _unitOfWork.TienDoHocRepository.Update(tienDo);
            }
            else
            {
                tienDo = new TienDoHoc
                {
                    NguoiDungId = userId,
                    BaiHocId = baiHocId,
                    DaHoanThanh = true,
                    ThoiDiemHoanThanh = DateTime.Now,
                    LastTimePosition = 0
                };
                await _unitOfWork.TienDoHocRepository.AddAsync(tienDo);
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}

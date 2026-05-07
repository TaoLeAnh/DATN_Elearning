using Elearning.Application.Interfaces;
using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Portal.Forms.KyThi;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elearning.Application.Services
{
    public class KyThiService : IKyThiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;

        public KyThiService(IUnitOfWork unitOfWork, IRequestContext requestContext)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
        }

        public async Task<DataTableJson> GetPaged(KyThiQuery searchOption)
        {
            var (items, total) = await _unitOfWork.KyThiRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<KyThiDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.KyThiRepository.FindAsync(
                x => x.Id == id,
                includes: new[] { "KhoaHoc", "CauHoiKyThis" }
            );

            if (entity == null)
                throw new ArgumentException("Không tìm thấy kỳ thi.");

            return new KyThiDto
            {
                Id = entity.Id,
                TenKyThi = entity.TenKyThi,

                // SỬA: Xử lý an toàn Null
                KhoaHocId = entity.KhoaHocId ?? Guid.Empty,
                TenKhoaHoc = entity.KhoaHoc?.TenKhoaHoc,
                ThoiGianBatDau = entity.ThoiGianBatDau ?? DateTime.MinValue,
                ThoiGianKetThuc = entity.ThoiGianKetThuc ?? DateTime.MinValue,

                ThoiLuongPhut = entity.ThoiLuongPhut,
                Created = entity.Created,
                LastModified = entity.LastModified,

                // THÊM: Map các trường mới trả về DTO
                MonHoc = entity.MonHoc,
                LoaiDeThi = entity.LoaiDeThi,
                NamThi = entity.NamThi,
                TinhThanh = entity.TinhThanh,
                TenTruong = entity.TenTruong,
                IsPublic = entity.IsPublic,

                SoLuongCauHoi = entity.CauHoiKyThis?.Count ?? 0
            };
        }

        public async Task<Guid> CreateAsync(KyThiForm form)
        {
            // 1. Xác định điều kiện cần thời gian: Không public HOẶC là loại ThiLive
            bool requiresTime = !form.IsPublic || form.LoaiDeThi == EnumLoaiDeThi.DeThiLive;

            // 2. Validate thời gian nếu thuộc nhóm bắt buộc
            if (requiresTime && form.ThoiGianKetThuc <= form.ThoiGianBatDau)
                throw new ArgumentException("Thời gian kết thúc phải sau thời gian bắt đầu.");

            var entity = new KyThi
            {
                TenKyThi = form.TenKyThi,
                ThoiLuongPhut = form.ThoiLuongPhut,

                IsPublic = form.IsPublic,
                MonHoc = form.IsPublic ? form.MonHoc : null,
                LoaiDeThi = form.IsPublic ? form.LoaiDeThi : null,
                NamThi = form.IsPublic ? form.NamThi : null,
                TinhThanh = form.IsPublic ? form.TinhThanh : null,
                TenTruong = form.IsPublic ? form.TenTruong : null,

                // Khóa học thì chắc chắn chỉ dành cho Nội bộ
                KhoaHocId = form.IsPublic ? null : form.KhoaHocId,

                // 3. FIX Ở ĐÂY: Lưu thời gian nếu requiresTime = true, nếu không thì dọn dẹp bằng null
                ThoiGianBatDau = requiresTime ? form.ThoiGianBatDau : null,
                ThoiGianKetThuc = requiresTime ? form.ThoiGianKetThuc : null
            };

            await _unitOfWork.KyThiRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(_requestContext.CurrentIdUser);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, KyThiForm form)
        {
            // 1. Xác định điều kiện cần thời gian
            bool requiresTime = !form.IsPublic || form.LoaiDeThi == EnumLoaiDeThi.DeThiLive;

            // 2. FIX LỖI NHỎ Ở ĐÂY: Dùng requiresTime để validate cho cả đề Public dạng Thi Live
            if (requiresTime && form.ThoiGianKetThuc <= form.ThoiGianBatDau)
                throw new ArgumentException("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");

            var entity = await _unitOfWork.KyThiRepository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.TenKyThi = form.TenKyThi;
            entity.ThoiLuongPhut = form.ThoiLuongPhut;

            // Cập nhật các trường mới
            entity.IsPublic = form.IsPublic;
            entity.MonHoc = form.IsPublic ? form.MonHoc : null;
            entity.LoaiDeThi = form.IsPublic ? form.LoaiDeThi : null;
            entity.NamThi = form.IsPublic ? form.NamThi : null;
            entity.TinhThanh = form.IsPublic ? form.TinhThanh : null;
            entity.TenTruong = form.IsPublic ? form.TenTruong : null;

            // Dọn rác dữ liệu
            entity.KhoaHocId = form.IsPublic ? null : form.KhoaHocId;
            entity.ThoiGianBatDau = requiresTime ? form.ThoiGianBatDau : null;
            entity.ThoiGianKetThuc = requiresTime ? form.ThoiGianKetThuc : null;

            _unitOfWork.KyThiRepository.Update(entity);
            await _unitOfWork.CompleteAsync(_requestContext.CurrentIdUser);

            return true;
        }

        //public async Task<bool> DeleteAsync(Guid id)
        //{
        //    var entity = await _unitOfWork.KyThiRepository.GetByIdAsync(id);
        //    if (entity == null) return false;

        //    _unitOfWork.KyThiRepository.Delete(entity);
        //    await _unitOfWork.CompleteAsync(_requestContext.CurrentIdUser);
        //    return true;
        //}
        public async Task<bool> DeleteAsync(Guid id)
        {
            // 1. Tìm bản ghi Kỳ thi cần xóa
            var entity = await _unitOfWork.KyThiRepository.GetByIdAsync(id);
            if (entity == null) return false;

            // 2. THỰC HIỆN XÓA MỀM: Chuyển trạng thái sang Cancelled (Hủy hoặc Xóa)
            // Lưu ý: Kiểm tra lại xem thuộc tính trên Entity KyThi của bác tên là ModerationStatus hay ModerStatus nhé
            entity.ModerationStatus = Elearning.Shared.Commons.Model.SQL.ModerationStatus.Cancelled;

            // 3. Thay vì gọi .Delete(), ta gọi .Update() để lưu trạng thái mới
            _unitOfWork.KyThiRepository.Update(entity);
            await _unitOfWork.CompleteAsync(_requestContext.CurrentIdUser);

            return true;
        }

        public async Task<List<CauHoiKyThiDto>> GetCauHinhDeThiAsync(Guid kyThiId)
        {
            var query = _unitOfWork.CauHoiKyThiRepository
                                   .GetTableNoTracking()
                                   .Include(x => x.CauHoi)
                                   .Where(x => x.KyThiId == kyThiId)
                                   .OrderBy(x => x.PhanThi).ThenBy(x => x.ThuTu);

            var entities = await query.ToListAsync();

            return entities.Select(x => new CauHoiKyThiDto
            {
                Id = x.Id,
                KyThiId = x.KyThiId,
                CauHoiId = x.CauHoiId,
                PhanThi = x.PhanThi,
                ThuTu = x.ThuTu,
                NoiDungCauHoi = x.CauHoi?.NoiDung ?? "[Ảnh/Video]"
            }).ToList();
        }

        public async Task<bool> SaveCauHinhDeThiAsync(Guid kyThiId, CauHinhDeThiForm form)
        {
            var repo = _unitOfWork.CauHoiKyThiRepository;

            // 1. Lấy danh sách cấu hình hiện tại đang có trong Database
            var existingItems = await repo.GetTableAsTracking()
                                          .Where(x => x.KyThiId == kyThiId)
                                          .ToListAsync();

            var existingCauHoiIds = existingItems.Select(x => x.CauHoiId).ToList();
            var newCauHoiIds = form.DanhSachCauHoi.Select(x => x.CauHoiId).ToList();

            // 2. XÓA: Lọc ra những câu hỏi có trong DB nhưng KHÔNG CÓ trong form gửi lên
            var itemsToDelete = existingItems.Where(x => !newCauHoiIds.Contains(x.CauHoiId)).ToList();
            if (itemsToDelete.Any())
            {
                repo.DeleteRange(itemsToDelete);
            }

            // 3. THÊM MỚI: Lọc ra những câu hỏi có trong form nhưng CHƯA CÓ trong DB
            var itemsToAdd = form.DanhSachCauHoi
                                 .Where(x => !existingCauHoiIds.Contains(x.CauHoiId))
                                 .Select(x => new CauHoiKyThi
                                 {
                                     KyThiId = kyThiId,
                                     CauHoiId = x.CauHoiId,
                                     PhanThi = x.PhanThi,
                                     ThuTu = x.ThuTu
                                 }).ToList();
            if (itemsToAdd.Any())
            {
                await repo.AddRangeAsync(itemsToAdd);
            }

            // 4. CẬP NHẬT: Lọc ra những câu hỏi TỒN TẠI ở cả 2 bên (để lỡ có đổi Phần thi hoặc Thứ tự)
            var itemsToUpdate = existingItems.Where(x => newCauHoiIds.Contains(x.CauHoiId)).ToList();
            foreach (var item in itemsToUpdate)
            {
                // Tìm dữ liệu tương ứng từ form để đè lên
                var match = form.DanhSachCauHoi.First(x => x.CauHoiId == item.CauHoiId);

                item.PhanThi = match.PhanThi;
                item.ThuTu = match.ThuTu;

                repo.Update(item);
            }

            // 5. Commit dữ liệu xuống DB
            await _unitOfWork.CompleteAsync(_requestContext.CurrentIdUser);

            return true;
        }

        public async Task<bool> GenerateRandomExamAsync(Guid kyThiId, MaTranDeThiForm maTran)
        {
            var repoCauHoi = _unitOfWork.CauHoiRepository;
            var repoCauHoiKyThi = _unitOfWork.CauHoiKyThiRepository;

            // 1. Xóa trắng đề thi cũ (nếu có) trước khi tạo mới
            var oldItems = await repoCauHoiKyThi.GetTableAsTracking()
                                                .Where(x => x.KyThiId == kyThiId)
                                                .ToListAsync();
            if (oldItems.Any()) repoCauHoiKyThi.DeleteRange(oldItems);

            var finalQuestions = new List<CauHoiKyThi>();

            var thuTuDict = new Dictionary<EnumLoaiPhanThi, int>
        {
            { EnumLoaiPhanThi.TracNghiem, 1 },
            { EnumLoaiPhanThi.MenhDeDungSai, 1 },
            { EnumLoaiPhanThi.DienKetQua, 1 }
        };

            // Lấy thông tin Kỳ thi hiện tại
            var kyThi = await _unitOfWork.KyThiRepository.GetByIdAsync(kyThiId);
            if (kyThi == null) return false;

            // =========================================================
            // BƯỚC 1.5: CẬP NHẬT LOẠI ĐỀ THI THÀNH "ĐỀ NGẪU NHIÊN"
            // =========================================================
            kyThi.LoaiDeThi = EnumLoaiDeThi.DeThiNgauNhien;
            _unitOfWork.KyThiRepository.Update(kyThi);

            // 2. Chạy qua từng "Luật" trong Ma trận để bốc câu hỏi
            foreach (var luat in maTran.DanhSachLuat)
            {
                if (luat.SoLuongCanLay <= 0) continue;

                // Lọc cơ bản theo Loại câu, Mức độ, Chủ đề
                var queryCauHoi = repoCauHoi.GetTableNoTracking()
                                            .Where(x => x.LoaiCauHoi == luat.LoaiCauHoiGoc
                                                     && x.MucDo == luat.MucDo
                                                     && x.ChuDe.ToLower() == luat.ChuDe.ToLower());

                if (!kyThi.IsPublic && kyThi.KhoaHocId.HasValue)
                {
                    queryCauHoi = queryCauHoi.Where(x => x.KhoaHocId == kyThi.KhoaHocId);
                }
                else if (kyThi.IsPublic && kyThi.MonHoc.HasValue)
                {
                    queryCauHoi = queryCauHoi.Where(x => x.MonHoc == kyThi.MonHoc);
                }
                else if (kyThi.IsPublic && !kyThi.MonHoc.HasValue)
                {
                    throw new Exception("Đề thi công khai (Public) bắt buộc phải cấu hình Môn học trước khi tạo đề ngẫu nhiên.");
                }

                var cauHois = await queryCauHoi.OrderBy(x => Guid.NewGuid())
                                               .Take(luat.SoLuongCanLay)
                                               .Select(x => x.Id)
                                               .ToListAsync();

                // Kiểm tra xem Ngân hàng có đủ câu để bốc không?
                if (cauHois.Count < luat.SoLuongCanLay)
                {
                    string dkLoc = !kyThi.IsPublic ? $"thuộc Khóa học '{kyThi.TenKyThi}'" : $"thuộc Môn '{kyThi.MonHoc}'";
                    throw new Exception($"Ngân hàng không đủ câu hỏi {dkLoc} cho Chủ đề '{luat.ChuDe}', Mức độ '{luat.MucDo}'. Cần {luat.SoLuongCanLay}, nhưng chỉ có {cauHois.Count}.");
                }

                // Đưa vào danh sách cuối cùng
                foreach (var cauHoiId in cauHois)
                {
                    finalQuestions.Add(new CauHoiKyThi
                    {
                        KyThiId = kyThiId,
                        CauHoiId = cauHoiId,
                        PhanThi = luat.PhanThi,
                        ThuTu = thuTuDict[luat.PhanThi]++
                    });
                }
            }

            // 3. Lưu toàn bộ (bao gồm cả danh sách câu hỏi mới VÀ trạng thái LoaiDeThi vừa cập nhật)
            await repoCauHoiKyThi.AddRangeAsync(finalQuestions);
            await _unitOfWork.CompleteAsync(_requestContext.CurrentIdUser);

            return true;
        }
    }
}
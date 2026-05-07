using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Commons.Extensions;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elearning.Publising.Application.Services
{
    public class KyThiService : IKyThiService
    {
        private readonly IUnitOfWorkPublising _UnitOfWork;
        private readonly IExamQueueService _examQueue;
        public KyThiService(IUnitOfWorkPublising unitOfWork, IExamQueueService examQueue)
        {
            _UnitOfWork = unitOfWork;
            _examQueue = examQueue;
        }

        public async Task<List<PublicKyThiDto>> GetPublicExamsAsync(MonHocEnum? monHoc = null)
        {
            var query = _UnitOfWork.KyThiRepository.GetTableNoTracking()
                .Where(x => x.IsPublic) // Bắt buộc chỉ lấy đề Public
                .Where(x => x.ModerationStatus != Elearning.Shared.Commons.Model.SQL.ModerationStatus.Cancelled);
            if (monHoc.HasValue)
            {
                query = query.Where(x => x.MonHoc == monHoc.Value);
            }

            // Sắp xếp mới nhất lên đầu
            var result = await query.OrderByDescending(x => x.Created)
                .Select(x => new PublicKyThiDto
                {
                    Id = x.Id,
                    TenKyThi = x.TenKyThi,
                    ThoiLuongPhut = x.ThoiLuongPhut,
                    SoLuongCauHoi = x.CauHoiKyThis.Count(),
                    MonHoc = x.MonHoc,
                    LoaiDeThi = x.LoaiDeThi,
                    ThoiGianBatDau = x.ThoiGianBatDau,
                    ThoiGianKetThuc = x.ThoiGianKetThuc,
                    NamThi = x.NamThi,
                    TinhThanh = x.TinhThanh
                }).ToListAsync();
            return result;
        }

        // =========================================================================
        // ĐÃ CẬP NHẬT: TẠO ĐỀ NGẪU NHIÊN TỪ MA TRẬN TRONG DATABASE
        // =========================================================================
        public async Task<Guid> GenerateRandomExamAsync(MonHocEnum monHoc)
        {
            // 1. Lấy Ma trận mặc định đang Active của Môn học từ DB
            var maTranDb = await _UnitOfWork.MaTranDeThiMacDinhRepository.GetTableNoTracking()
                .Include(x => x.ChiTiets)
                .FirstOrDefaultAsync(x => x.MonHoc == monHoc && x.IsActive);

            if (maTranDb == null || !maTranDb.ChiTiets.Any())
            {
                throw new Exception($"Quản trị viên chưa cấu hình Ma trận đề thi mặc định cho môn {monHoc.GetDescription()}.");
            }

            // 2. Tạo 1 Kỳ thi dạng Ẩn (IsPublic = false) dành riêng cho phiên luyện tập
            var kyThi = new KyThi
            {
                TenKyThi = $"Đề luyện tập {monHoc.GetDescription()} - {DateTime.Now:dd/MM/yyyy HH:mm}",
                ThoiLuongPhut = GetThoiLuongThi(monHoc), 
                IsPublic = false,
                MonHoc = monHoc,
                LoaiDeThi = EnumLoaiDeThi.DeThiNgauNhien
            };

            await _UnitOfWork.KyThiRepository.AddAsync(kyThi);
            await _UnitOfWork.CompleteAsync(); // Lưu để lấy kyThi.Id

            // 3. Tiến hành bốc câu hỏi từ Ngân hàng dựa trên Ma trận
            var repoCauHoi = _UnitOfWork.CauHoiRepository;
            var finalQuestions = new List<CauHoiKyThi>();

            var thuTuDict = new Dictionary<EnumLoaiPhanThi, int>
            {
                { EnumLoaiPhanThi.TracNghiem, 1 },
                { EnumLoaiPhanThi.MenhDeDungSai, 1 },
                { EnumLoaiPhanThi.DienKetQua, 1 }
            };

            foreach (var chiTiet in maTranDb.ChiTiets)
            {
                if (chiTiet.SoLuong <= 0) continue;

                // Lọc cơ bản theo Môn, Loại câu, Mức độ
                var queryCauHoi = repoCauHoi.GetTableNoTracking()
                    .Where(x => x.MonHoc == monHoc
                             && x.LoaiCauHoi == chiTiet.LoaiCauHoi
                             && x.MucDo == chiTiet.MucDo);

                // Lọc theo Chủ đề (nếu có nhập)
                if (!string.IsNullOrWhiteSpace(chiTiet.ChuDe) && chiTiet.ChuDe != "Tổng hợp")
                {
                    queryCauHoi = queryCauHoi.Where(x => x.ChuDe.ToLower() == chiTiet.ChuDe.ToLower());
                }

                var cauHois = await queryCauHoi.OrderBy(x => Guid.NewGuid()) // RANDOM CỦA SQL
                                               .Take(chiTiet.SoLuong)
                                               .Select(x => x.Id)
                                               .ToListAsync();

                // Kiểm tra xem Ngân hàng có đủ câu để bốc không
                if (cauHois.Count < chiTiet.SoLuong)
                {
                    string thongBaoChuDe = string.IsNullOrWhiteSpace(chiTiet.ChuDe) ? "chung" : chiTiet.ChuDe;
                    throw new Exception($"Ngân hàng không đủ câu hỏi môn '{monHoc.GetDescription()}' cho Chủ đề '{thongBaoChuDe}', Mức độ '{chiTiet.MucDo.GetDescription()}'. Cần {chiTiet.SoLuong}, nhưng chỉ có {cauHois.Count}.");
                }

                // Đưa vào danh sách cuối cùng
                foreach (var cauHoiId in cauHois)
                {
                    if (!thuTuDict.ContainsKey(chiTiet.PhanThi)) thuTuDict[chiTiet.PhanThi] = 1;

                    finalQuestions.Add(new CauHoiKyThi
                    {
                        KyThiId = kyThi.Id,
                        CauHoiId = cauHoiId,
                        PhanThi = chiTiet.PhanThi,
                        ThuTu = thuTuDict[chiTiet.PhanThi]++
                    });
                }
            }

            // 4. Lưu toàn bộ các câu hỏi đã bốc vào Database
            await _UnitOfWork.CauHoiKyThiRepository.AddRangeAsync(finalQuestions);
            await _UnitOfWork.CompleteAsync();

            return kyThi.Id;
        }

        public async Task<BoCauHoiOnTapDto> GetDeThiLamBaiAsync(Guid kyThiId)
        {
            var kyThi = await _UnitOfWork.KyThiRepository.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == kyThiId);
            if (kyThi == null) return null;

            // Kéo toàn bộ câu hỏi và đáp án của đề thi này lên
            var cauHois = await _UnitOfWork.CauHoiKyThiRepository.GetTableNoTracking()
                .Include(x => x.CauHoi).ThenInclude(c => c.DapAns)
                .Include(x => x.CauHoi).ThenInclude(c => c.MenhDeDungSais)
                .Include(x => x.CauHoi).ThenInclude(c => c.DapAnDienKetQuas)
                .Where(x => x.KyThiId == kyThiId)
                .OrderBy(x => x.PhanThi).ThenBy(x => x.ThuTu)
                .ToListAsync();

            var dto = new BoCauHoiOnTapDto
            {
                Id = kyThi.Id,
                TenBo = kyThi.TenKyThi,
                MoTa = $"Thời gian làm bài: {kyThi.ThoiLuongPhut} phút",
                ThoiLuongPhut = kyThi.ThoiLuongPhut,
                ChiTietBoCauHois = cauHois.Select(ch => new ChiTietBoCauHoiDto
                {
                    ThuTu = ch.ThuTu,
                    CauHoiId = ch.CauHoiId,
                    NoiDungCauHoi = ch.CauHoi.NoiDung,
                    HinhAnhUrlCauHoi = ch.CauHoi.HinhAnhUrl,
                    DapAns = ch.CauHoi.DapAns.Select(d => new DapAnDto { Id = d.Id, NoiDung = d.NoiDung, HinhAnhUrl = d.HinhAnhUrl, ThuTu = d.ThuTu }).ToList(),
                    MenhDeDungSais = ch.CauHoi.MenhDeDungSais.Select(m => new MenhDeDungSaiDto { Id = m.Id, NoiDung = m.NoiDung, ThuTu = m.ThuTu }).ToList(),
                    DapAnDienKetQuas = ch.CauHoi.DapAnDienKetQuas.Select(dk => new DapAnDienKetQuaDto { Id = dk.Id }).ToList()
                }).ToList()
            };

            return dto;
        }

        // =========================================================================
        // MODULE GIÁM THỊ ẢO & CHẤM ĐIỂM
        // =========================================================================

        public async Task<Guid> BatDauThiAsync(Guid kyThiId, Guid userId)
        {
            var kyThi = await _UnitOfWork.KyThiRepository.GetByIdAsync(kyThiId);
            if (kyThi == null) throw new Exception("Không tìm thấy kỳ thi.");

            var baiLam = new BaiLam
            {
                KyThiId = kyThiId,
                NguoiDungId = userId,
                ThoiDiemBatDau = DateTime.Now,
                TrangThai = EnumTrangThaiBaiLam.DangLam, // ĐÃ FIX: Không dùng số cứng
                Diem = 0,
                SoCauDung = 0
            };

            await _UnitOfWork.BaiLamRepository.AddAsync(baiLam);
            await _UnitOfWork.CompleteAsync();

            return baiLam.Id;
        }

        public async Task<bool> GhiNhanViPhamRealTimeAsync(Guid baiLamId, EnumLoaiViPham loai, string chiTiet)
        {
            var log = new LogViPham
            {
                BaiLamId = baiLamId,
                LoaiViPham = loai,
                ThoiDiemViPham = DateTime.Now,
                ChiTiet = chiTiet
            };

            await _UnitOfWork.LogViPhamRepository.AddAsync(log);
            await _UnitOfWork.CompleteAsync();
            return true;
        }

        public async Task<float> NopBaiThiAsync(NopBaiRequest payload)
        {
            var kyThi = await _UnitOfWork.KyThiRepository.GetByIdAsync(payload.BoCauHoiId);
            if (kyThi == null) return 0;

            var cauHois = await _UnitOfWork.CauHoiKyThiRepository.GetTableNoTracking()
                .Include(x => x.CauHoi).ThenInclude(c => c.DapAns)
                .Where(x => x.KyThiId == payload.BoCauHoiId)
                .ToListAsync();

            int soCauDung = 0;
            int tongSoCau = cauHois.Count;
            if (tongSoCau == 0) return 0;

            // Chấm điểm cơ bản (Câu hỏi trắc nghiệm 1 lựa chọn)
            foreach (var cauHoiThi in cauHois)
            {
                var traLoi = payload.DanhSachTraLoi.FirstOrDefault(x => x.CauHoiId == cauHoiThi.CauHoiId);
                if (traLoi == null) continue;

                var dapAnDung = cauHoiThi.CauHoi.DapAns.FirstOrDefault(x => x.LaDapAnDung);
                if (dapAnDung != null && traLoi.DapAnId == dapAnDung.Id)
                {
                    soCauDung++;
                }
            }

            float diem = (float)Math.Round(((double)soCauDung / tongSoCau) * 10, 2);

            if (payload.UserId != Guid.Empty)
            {
                // TÌM LẠI BẢN GHI BÀI LÀM ĐANG THI DỞ
                var baiLam = await _UnitOfWork.BaiLamRepository.GetTableNoTracking()
                    .Where(x => x.KyThiId == kyThi.Id && x.NguoiDungId == payload.UserId && x.TrangThai != EnumTrangThaiBaiLam.DaNop)
                    .OrderByDescending(x => x.ThoiDiemBatDau)
                    .FirstOrDefaultAsync();

                if (baiLam != null)
                {
                    baiLam.Diem = diem;
                    baiLam.SoCauDung = soCauDung;
                    baiLam.ThoiDiemNop = DateTime.Now;
                    baiLam.TrangThai = EnumTrangThaiBaiLam.DaNop; // ĐÃ FIX

                    _UnitOfWork.BaiLamRepository.Update(baiLam);
                }
                else
                {
                    baiLam = new BaiLam
                    {
                        KyThiId = kyThi.Id,
                        NguoiDungId = payload.UserId,
                        Diem = diem,
                        SoCauDung = soCauDung,
                        ThoiDiemBatDau = DateTime.Now.AddMinutes(-kyThi.ThoiLuongPhut),
                        ThoiDiemNop = DateTime.Now,
                        TrangThai = EnumTrangThaiBaiLam.DaNop // ĐÃ FIX
                    };
                    await _UnitOfWork.BaiLamRepository.AddAsync(baiLam);
                }

                await _UnitOfWork.CompleteAsync();
            }

            return diem;
        }
        public async Task<(bool, string)> DayBaiNopVaoQueueAsync(NopBaiRequest request)
        {
            var baiLam = await _UnitOfWork.BaiLamRepository.GetByIdAsync(request.BaiLamId);
            if (baiLam != null)
            {
                baiLam.TrangThai = EnumTrangThaiBaiLam.DaNop;
                await _UnitOfWork.CompleteAsync();
            }
            var queueItem = new ExamQueueItem
            {
                Request = request,
                RetryCount = 0 
            };
            await _examQueue.EnqueueBaiNopAsync(queueItem);

            return (true, "Đã đưa vào hàng đợi chấm điểm");
        }
        private int GetThoiLuongThi(MonHocEnum monHoc)
        {
            return monHoc switch
            {
                MonHocEnum.NguVan => 120,  // Ngữ văn thi tự luận 120 phút
                MonHocEnum.Toan => 90,  // Toán thi trắc nghiệm 90 phút
                _ => 50                    // Tất cả các môn trắc nghiệm còn lại 50 phút
            };
        }
    }

}
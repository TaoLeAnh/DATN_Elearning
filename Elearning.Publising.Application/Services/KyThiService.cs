using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Commons.Extensions;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
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

                    // 👉 THÊM ĐÚNG DÒNG NÀY ĐỂ UI BIẾT ĐƯỜNG MÀ CHIA 3 PHẦN NÈ BÁC:
                    LoaiCauHoi = ch.CauHoi.LoaiCauHoi,

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

            // Phải Include cả MenhDe và DienKetQua để chấm điểm
            var cauHois = await _UnitOfWork.CauHoiKyThiRepository.GetTableNoTracking()
                .Include(x => x.CauHoi).ThenInclude(c => c.DapAns)
                .Include(x => x.CauHoi).ThenInclude(c => c.MenhDeDungSais)
                .Include(x => x.CauHoi).ThenInclude(c => c.DapAnDienKetQuas)
                .Where(x => x.KyThiId == payload.BoCauHoiId)
                .ToListAsync();

            int soCauDung = 0;
            int tongSoCau = cauHois.Count;
            if (tongSoCau == 0) return 0;

            // TÍNH ĐIỂM FULL 3 PHẦN THI
            foreach (var cauHoiThi in cauHois)
            {
                var traLoi = payload.DanhSachTraLoi.FirstOrDefault(x => x.CauHoiId == cauHoiThi.CauHoiId);
                if (traLoi == null) continue;

                if (cauHoiThi.PhanThi == EnumLoaiPhanThi.TracNghiem)
                {
                    var dapAnDung = cauHoiThi.CauHoi.DapAns.FirstOrDefault(x => x.LaDapAnDung);
                    if (dapAnDung != null && traLoi.DapAnId == dapAnDung.Id)
                        soCauDung++;
                }
                else if (cauHoiThi.PhanThi == EnumLoaiPhanThi.MenhDeDungSai)
                {
                    int soYChinhXac = 0;
                    foreach (var md in cauHoiThi.CauHoi.MenhDeDungSais)
                    {
                        var luaChonSV = traLoi.MenhDes.FirstOrDefault(x => x.MenhDeId == md.Id);
                        if (luaChonSV != null && luaChonSV.LuaChonCuaHocVien == md.LaDung)
                            soYChinhXac++;
                    }
                    if (soYChinhXac == cauHoiThi.CauHoi.MenhDeDungSais.Count && soYChinhXac > 0)
                        soCauDung++;
                }
                else if (cauHoiThi.PhanThi == EnumLoaiPhanThi.DienKetQua)
                {
                    var dapAnDung = cauHoiThi.CauHoi.DapAnDienKetQuas.FirstOrDefault();
                    if (dapAnDung != null && traLoi.GiaTriNhap.HasValue)
                    {
                        float saiSo = Math.Abs(traLoi.GiaTriNhap.Value - dapAnDung.GiaTriDung);
                        if (saiSo <= dapAnDung.SaiSoChoPhep)
                            soCauDung++;
                    }
                }
            }

            // Công thức tính điểm hệ 10
            float diem = (float)Math.Round(((double)soCauDung / tongSoCau) * 10, 2);

            // ==============================================================
            // XỬ LÝ LƯU DATABASE (CHỐNG LỖI MẤT SESSION VÀ LƯU CHI TIẾT BÀI LÀM)
            // ==============================================================
            BaiLam baiLam = null;

            // 1. Tìm Bài làm theo ID JS gửi lên
            // 1. Tìm Bài làm theo ID JS gửi lên
            if (payload.BaiLamId != Guid.Empty)
            {
                // 👉 ĐỔI GetTable() THÀNH GetTableNoTracking()
                baiLam = await _UnitOfWork.BaiLamRepository.GetTableNoTracking()
                    .Include(x => x.ChiTietBaiLams)
                    .FirstOrDefaultAsync(x => x.Id == payload.BaiLamId);
            }

            // 2. DỰ PHÒNG CHỐNG LỖI: Nếu JS mất BaiLamId (do F5 trang), tìm bài thi Đang Làm của User này
            if (baiLam == null && payload.UserId != Guid.Empty)
            {
                // 👉 ĐỔI GetTable() THÀNH GetTableNoTracking()
                baiLam = await _UnitOfWork.BaiLamRepository.GetTableNoTracking()
                    .Include(x => x.ChiTietBaiLams)
                    .Where(x => x.KyThiId == kyThi.Id && x.NguoiDungId == payload.UserId && x.TrangThai == EnumTrangThaiBaiLam.DangLam)
                    .OrderByDescending(x => x.ThoiDiemBatDau)
                    .FirstOrDefaultAsync();
            }

            // 3. NẾU VẪN KHÔNG TÌM THẤY -> Tạo mới tinh (Trường hợp gọi thẳng API không qua nút Bắt đầu)
            bool isCreateNew = false;
            if (baiLam == null)
            {
                baiLam = new BaiLam
                {
                    KyThiId = kyThi.Id,
                    NguoiDungId = payload.UserId, // ĐẢM BẢO LUÔN CÓ TÊN NGƯỜI NỘP
                    ThoiDiemBatDau = DateTime.Now.AddMinutes(-kyThi.ThoiLuongPhut)
                };
                isCreateNew = true;
            }

            // 4. Cập nhật Điểm số & Trạng thái
            baiLam.Diem = diem;
            baiLam.SoCauDung = soCauDung;
            baiLam.ThoiDiemNop = DateTime.Now;
            baiLam.TrangThai = EnumTrangThaiBaiLam.DaNop; // Chờ duyệt

            // 5. Làm sạch đáp án cũ (nếu có thi lại/nộp đè)
            if (!isCreateNew && baiLam.ChiTietBaiLams.Any())
            {
                baiLam.ChiTietBaiLams.Clear();
            }

            // 6. LƯU TỪNG CÂU TRẢ LỜI CỦA SINH VIÊN VÀO BẢNG CHITIETBAILAM
            foreach (var traLoi in payload.DanhSachTraLoi)
            {
                var chiTiet = new ChiTietBaiLam
                {
                    CauHoiId = traLoi.CauHoiId,
                    DapAnId = traLoi.DapAnId,
                    GiaTriNhap = traLoi.GiaTriNhap
                };

                // Nếu là Phần 2 (Mệnh đề Đúng/Sai), phải lưu vào List con
                if (traLoi.MenhDes != null && traLoi.MenhDes.Any())
                {
                    foreach (var md in traLoi.MenhDes)
                    {
                        chiTiet.ChiTietTraLoiMenhDes.Add(new ChiTietTraLoiMenhDe
                        {
                            MenhDeDungSaiId = md.MenhDeId,
                            LuaChonCuaHocVien = md.LuaChonCuaHocVien
                        });
                    }
                }

                baiLam.ChiTietBaiLams.Add(chiTiet);
            }

            // 7. Hoàn tất giao dịch
            if (isCreateNew)
                await _UnitOfWork.BaiLamRepository.AddAsync(baiLam);
            else
                _UnitOfWork.BaiLamRepository.Update(baiLam);

            await _UnitOfWork.CompleteAsync();

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
        public async Task<List<BaiLamDto>> GetMyExamsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Không có ID người dùng");

            // Chỉ lấy những bài đã được Giảng viên Duyệt (DaCham)
            var query = await _UnitOfWork.BaiLamRepository.GetTableNoTracking()
                .Include(x => x.KyThi)
                .Include(x => x.ChiTietBaiLams)
                .Where(x => x.NguoiDungId == userId && x.TrangThai == EnumTrangThaiBaiLam.DaCham)
                .OrderByDescending(x => x.ThoiDiemNop)
                .Select(x => new BaiLamDto
                {
                    Id = x.Id,
                    KyThiId = x.KyThiId,
                    TenKyThi = x.KyThi != null ? x.KyThi.TenKyThi : "Không xác định",
                    MonHoc = (x.KyThi != null && x.KyThi.MonHoc.HasValue) ? x.KyThi.MonHoc.ToString() : string.Empty,
                    ThoiDiemBatDau = x.ThoiDiemBatDau,
                    ThoiDiemNop = x.ThoiDiemNop,
                    Diem = x.Diem,
                    SoCauDung = x.SoCauDung,
                    TongSoCau = x.ChiTietBaiLams.Count()
                }).ToListAsync();

            return query;
        }
        public async Task<BaiLamReviewDto> GetChiTietBaiLamHocVienAsync(Guid baiLamId, Guid userId)
        {
            // 1. Chỉ lấy bài làm nếu ĐÚNG LÀ CỦA USER ĐÓ (Bảo mật 100%)
            var baiLam = await _UnitOfWork.BaiLamRepository.GetTableNoTracking()
                .Include(x => x.KyThi)
                .Include(x => x.NguoiDung)
                .FirstOrDefaultAsync(x => x.Id == baiLamId && x.NguoiDungId == userId);

            if (baiLam == null)
                throw new ArgumentException("Không tìm thấy bài làm hoặc bạn không có quyền xem.");

            // 2. Kéo toàn bộ đề gốc
            var deThiGoc = await _UnitOfWork.CauHoiKyThiRepository.GetTableNoTracking()
                .Include(x => x.CauHoi).ThenInclude(c => c.DapAns)
                .Include(x => x.CauHoi).ThenInclude(c => c.MenhDeDungSais)
                .Include(x => x.CauHoi).ThenInclude(c => c.DapAnDienKetQuas)
                .Where(x => x.KyThiId == baiLam.KyThiId)
                .OrderBy(x => x.PhanThi).ThenBy(x => x.ThuTu)
                .ToListAsync();

            // 3. Lấy chi tiết bài làm của sinh viên
            var chiTietBaiLamSV = await _UnitOfWork.ChiTietBaiLamRepository.GetTableNoTracking()
                .Include(x => x.ChiTietTraLoiMenhDes)
                .Where(x => x.BaiLamId == baiLamId)
                .ToListAsync();

            // 4. Map Dữ liệu
            var reviewDto = new BaiLamReviewDto
            {
                BaiLamId = baiLam.Id,
                KyThiId = baiLam.KyThiId ?? Guid.Empty,
                TenKyThi = baiLam.KyThi?.TenKyThi ?? "N/A",
                TenSinhVien = baiLam.NguoiDung?.Ten ?? "Học viên",
                MonHoc = baiLam.KyThi != null && baiLam.KyThi.MonHoc.HasValue ? baiLam.KyThi.MonHoc.ToString() : null,
                Diem = baiLam.Diem,
                SoCauDung = baiLam.SoCauDung,
                TongSoCau = deThiGoc.Count,
                ThoiDiemBatDau = baiLam.ThoiDiemBatDau,
                ThoiDiemNop = baiLam.ThoiDiemNop
            };

            // 5. Trộn đáp án để trả về UI
            foreach (var cauKyThi in deThiGoc)
            {
                var cauHoi = cauKyThi.CauHoi;
                var traLoiSV = chiTietBaiLamSV.FirstOrDefault(x => x.CauHoiId == cauKyThi.CauHoiId);

                var cauHoiDto = new CauHoiReviewDto
                {
                    CauHoiId = cauKyThi.CauHoiId,
                    ThuTu = cauKyThi.ThuTu,
                    PhanThi = cauKyThi.PhanThi,
                    NoiDungCauHoi = cauHoi.NoiDung ?? "",
                    GiaiThich = cauHoi.GiaiThich,
                    IsCorrect = false
                };

                if (cauKyThi.PhanThi == EnumLoaiPhanThi.TracNghiem)
                {
                    cauHoiDto.DapAns = cauHoi.DapAns.Select(d => new DapAnReviewDto { Id = d.Id, NoiDung = d.NoiDung, LaDapAnDung = d.LaDapAnDung }).ToList();
                    cauHoiDto.DapAnHocVienChonId = traLoiSV?.DapAnId;
                    var dapAnDung = cauHoi.DapAns.FirstOrDefault(x => x.LaDapAnDung);
                    if (dapAnDung != null && cauHoiDto.DapAnHocVienChonId == dapAnDung.Id) cauHoiDto.IsCorrect = true;
                }
                else if (cauKyThi.PhanThi == EnumLoaiPhanThi.MenhDeDungSai)
                {
                    int soYChinhXac = 0;
                    cauHoiDto.MenhDes = cauHoi.MenhDeDungSais.Select(md =>
                    {
                        var luaChonSV = traLoiSV?.ChiTietTraLoiMenhDes.FirstOrDefault(x => x.MenhDeDungSaiId == md.Id)?.LuaChonCuaHocVien;
                        if (luaChonSV.HasValue && luaChonSV.Value == md.LaDung) soYChinhXac++;

                        return new MenhDeReviewDto { Id = md.Id, NoiDung = md.NoiDung, LaDung = md.LaDung, LuaChonCuaHocVien = luaChonSV };
                    }).ToList();

                    if (soYChinhXac == cauHoi.MenhDeDungSais.Count && cauHoi.MenhDeDungSais.Count > 0) cauHoiDto.IsCorrect = true;
                }
                else if (cauKyThi.PhanThi == EnumLoaiPhanThi.DienKetQua)
                {
                    var dapAnDung = cauHoi.DapAnDienKetQuas.FirstOrDefault();
                    cauHoiDto.GiaTriDung = dapAnDung?.GiaTriDung;
                    cauHoiDto.SaiSoChoPhep = dapAnDung?.SaiSoChoPhep;
                    cauHoiDto.GiaTriHocVienNhap = traLoiSV?.GiaTriNhap;

                    if (dapAnDung != null && cauHoiDto.GiaTriHocVienNhap.HasValue)
                    {
                        float saiSo = Math.Abs(cauHoiDto.GiaTriHocVienNhap.Value - dapAnDung.GiaTriDung);
                        if (saiSo <= dapAnDung.SaiSoChoPhep) cauHoiDto.IsCorrect = true;
                    }
                }

                reviewDto.DanhSachCauHoi.Add(cauHoiDto);
            }

            return reviewDto;
        }
    }

}
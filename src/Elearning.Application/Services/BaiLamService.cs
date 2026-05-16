using Elearning.Application.Interfaces;
using Elearning.Domain.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using Microsoft.EntityFrameworkCore;

namespace Elearning.Application.Services
{
    public class BaiLamService : IBaiLamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _RequestContext;

        public BaiLamService(IUnitOfWork unitOfWork, IRequestContext requestContext)
        {
            _unitOfWork = unitOfWork;
            _RequestContext = requestContext;
        }
        public async Task<DataTableJson> GetPagedAdminAsync(BaiLamQuery searchOption)
        {
            // 👉 ĐÃ SỬA: Cho phép KyThiId rỗng NẾU đang tìm theo NguoiDungId (để lấy lịch sử)
            if (searchOption.KyThiId == Guid.Empty && !searchOption.NguoiDungId.HasValue)
                throw new ArgumentException("Yêu cầu cung cấp Mã kỳ thi hoặc Mã người dùng để xem danh sách.");

            var (items, total) = await _unitOfWork.BaiLamRepository.GetPagedDtoAsync(searchOption);

            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }
        public async Task<BaiLamReviewDto> GetChiTietBaiLamAsync(Guid baiLamId)
        {
            // 1. Lấy thông tin bài làm và Sinh viên
            var baiLam = await _unitOfWork.BaiLamRepository.GetTableNoTracking()
                .Include(x => x.NguoiDung)
                .Include(x => x.KyThi)
                .FirstOrDefaultAsync(x => x.Id == baiLamId);

            if (baiLam == null) throw new ArgumentException("Không tìm thấy bài làm.");

            // 2. Lấy toàn bộ đề thi gốc (Câu hỏi + Đáp án)
            var deThiGoc = await _unitOfWork.CauHoiKyThiRepository.GetTableNoTracking()
                .Include(x => x.CauHoi).ThenInclude(c => c.DapAns)
                .Include(x => x.CauHoi).ThenInclude(c => c.MenhDeDungSais)
                .Include(x => x.CauHoi).ThenInclude(c => c.DapAnDienKetQuas)
                .Where(x => x.KyThiId == baiLam.KyThiId)
                .OrderBy(x => x.PhanThi).ThenBy(x => x.ThuTu)
                .ToListAsync();

            // 3. Lấy chi tiết bài làm của sinh viên (SỬA LẠI: Gọi trực tiếp ChiTietBaiLamRepository)
            var chiTietBaiLamSV = await _unitOfWork.ChiTietBaiLamRepository.GetTableNoTracking()
                .Include(x => x.ChiTietTraLoiMenhDes) // Nhớ include cái bảng con của Phần 2
                .Where(x => x.BaiLamId == baiLamId)
                .ToListAsync();

            // 4. Map dữ liệu sang Review DTO
            var reviewDto = new BaiLamReviewDto
            {
                BaiLamId = baiLam.Id,

                // === BỔ SUNG 4 DÒNG NÀY VÀO ===
                KyThiId = baiLam.KyThiId ?? Guid.Empty,
                MaSinhVien = baiLam.NguoiDung?.Ten ?? "N/A", // Map mã sinh viên
                IsKyThiPublic = baiLam.KyThi != null && baiLam.KyThi.IsPublic,
                MonHoc = baiLam.KyThi != null && baiLam.KyThi.MonHoc.HasValue ? baiLam.KyThi.MonHoc.ToString() : null,
                // ==============================

                TenSinhVien = baiLam.NguoiDung?.Ten ?? "N/A",
                TenKyThi = baiLam.KyThi?.TenKyThi ?? "N/A",
                Diem = baiLam.Diem,
                SoCauDung = baiLam.SoCauDung,
                TongSoCau = deThiGoc.Count,
                ThoiDiemBatDau = baiLam.ThoiDiemBatDau,
                ThoiDiemNop = baiLam.ThoiDiemNop
            };

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
                    IsCorrect = false // Tạm set false, kiểm tra bên dưới
                };

                // Kẹp dữ liệu tùy theo Phần thi
                if (cauKyThi.PhanThi == EnumLoaiPhanThi.TracNghiem)
                {
                    cauHoiDto.DapAns = cauHoi.DapAns.Select(d => new DapAnReviewDto { Id = d.Id, NoiDung = d.NoiDung, LaDapAnDung = d.LaDapAnDung }).ToList();
                    cauHoiDto.DapAnHocVienChonId = traLoiSV?.DapAnId;

                    // Kiểm tra đúng sai cho UI
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

                        return new MenhDeReviewDto
                        {
                            Id = md.Id,
                            NoiDung = md.NoiDung,
                            LaDung = md.LaDung,
                            LuaChonCuaHocVien = luaChonSV
                        };
                    }).ToList();

                    // Nếu đúng cả 4 ý thì coi như câu này IsCorrect (tùy logic hiển thị của bạn)
                    if (soYChinhXac == 4) cauHoiDto.IsCorrect = true;
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
        public async Task<bool> DuyetBaiLamAsync(Guid baiLamId)
        {
            var baiLam = await _unitOfWork.BaiLamRepository.GetByIdAsync(baiLamId);
            if (baiLam == null)
                throw new ArgumentException("Không tìm thấy bài làm.");

            if (baiLam.TrangThai == EnumTrangThaiBaiLam.DaNop)
            {
                baiLam.TrangThai = EnumTrangThaiBaiLam.DaCham; // Đổi sang Đã chấm
                _unitOfWork.BaiLamRepository.Update(baiLam);
                await _unitOfWork.CompleteAsync();

                return true;
            }

            throw new ArgumentException("Bài làm chưa nộp hoặc đã được duyệt rồi.");
        }
    }
}

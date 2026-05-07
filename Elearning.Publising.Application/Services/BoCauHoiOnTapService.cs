using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Services
{
    public class BoCauHoiOnTapService : IBoCauHoiOnTapService
    {
        private readonly IUnitOfWorkPublising _unitOfWork;

        public BoCauHoiOnTapService(IUnitOfWorkPublising unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BoCauHoiOnTapDto> GetQuizDetailForStudentAsync(Guid id)
        {
            var entity = await _unitOfWork.BoCauHoiOnTapRepository.GetTableNoTracking()
                .Include(x => x.GiangVien)
                .Include(x => x.KhoaHoc)
                .Include(x => x.ChuongHoc)
                .Include(x => x.BaiHoc)
                .Include(x => x.ChiTietBoCauHois)
                    .ThenInclude(c => c.CauHoi)
                        .ThenInclude(ch => ch.DapAns)
                .Include(x => x.ChiTietBoCauHois)
                    .ThenInclude(c => c.CauHoi)
                        .ThenInclude(ch => ch.MenhDeDungSais)
                .Include(x => x.ChiTietBoCauHois)
                    .ThenInclude(c => c.CauHoi)
                        .ThenInclude(ch => ch.DapAnDienKetQuas)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            return new BoCauHoiOnTapDto
            {
                Id = entity.Id,
                TenBo = entity.TenBo,
                MoTa = entity.MoTa,
                LoaiBoCauHoi = entity.LoaiBoCauHoi,
                ThoiLuongPhut = entity.ThoiLuongPhut,
                KhoaHocId = entity.KhoaHocId,
                TenKhoaHoc = entity.KhoaHoc?.TenKhoaHoc,
                ChuongHocId = entity.ChuongHocId,
                TenChuongHoc = entity.ChuongHoc?.TenChuong,
                BaiHocId = entity.BaiHocId,
                TenBaiHoc = entity.BaiHoc?.TieuDe,
                GiangVienId = entity.GiangVienId,
                TenGiangVien = entity.GiangVien?.Ten,

                ChiTietBoCauHois = entity.ChiTietBoCauHois.Select(c => new ChiTietBoCauHoiDto
                {
                    Id = c.Id,
                    BoCauHoiOnTapId = c.BoCauHoiOnTapId,
                    CauHoiId = c.CauHoiId,
                    ThuTu = c.ThuTu,
                    LoaiCauHoi = c.CauHoi?.LoaiCauHoi ?? EnumLoaiCauHoi.MotLuaChon,
                    NoiDungCauHoi = c.CauHoi?.NoiDung,
                    HinhAnhUrlCauHoi = c.CauHoi?.HinhAnhUrl,

                    DapAns = c.CauHoi?.DapAns?.Select(d => new DapAnDto
                    {
                        Id = d.Id,
                        NoiDung = d.NoiDung,
                        ThuTu = d.ThuTu,
                        HinhAnhUrl = d.HinhAnhUrl
                    }).OrderBy(d => d.ThuTu).ToList() ?? new(),

                    MenhDeDungSais = c.CauHoi?.MenhDeDungSais?.Select(m => new MenhDeDungSaiDto { Id = m.Id, NoiDung = m.NoiDung, ThuTu = m.ThuTu }).OrderBy(m => m.ThuTu).ToList() ?? new(),
                    DapAnDienKetQuas = c.CauHoi?.DapAnDienKetQuas?.Select(dk => new DapAnDienKetQuaDto { Id = dk.Id }).ToList() ?? new()
                }).OrderBy(c => c.ThuTu).ToList()
            };
        }

        public async Task<float> NopBaiVaChamDiemAsync(NopBaiRequest request, Guid userId)
        {
            var boCauHoi = await _unitOfWork.BoCauHoiOnTapRepository.GetTableNoTracking()
                .Include(x => x.ChiTietBoCauHois).ThenInclude(c => c.CauHoi).ThenInclude(ch => ch.DapAns)
                .Include(x => x.ChiTietBoCauHois).ThenInclude(c => c.CauHoi).ThenInclude(ch => ch.MenhDeDungSais)
                .Include(x => x.ChiTietBoCauHois).ThenInclude(c => c.CauHoi).ThenInclude(ch => ch.DapAnDienKetQuas)
                .FirstOrDefaultAsync(x => x.Id == request.BoCauHoiId);

            if (boCauHoi == null) throw new Exception("Không tìm thấy bộ câu hỏi!");

            int soCauDung = 0;
            int tongSoCau = boCauHoi.ChiTietBoCauHois.Count;

            var baiLam = new BaiLam
            {
                BoCauHoiOnTapId = request.BoCauHoiId,
                NguoiDungId = userId,
                ThoiDiemBatDau = DateTime.Now.AddSeconds(-request.ThoiGianLamBaiGiay),
                ThoiDiemNop = DateTime.Now,
                TrangThai = EnumTrangThaiBaiLam.DaNop,
                ChiTietBaiLams = new List<ChiTietBaiLam>()
            };

            foreach (var traLoi in request.DanhSachTraLoi)
            {
                var cauHoiGoc = boCauHoi.ChiTietBoCauHois.FirstOrDefault(c => c.CauHoiId == traLoi.CauHoiId)?.CauHoi;
                if (cauHoiGoc == null) continue;

                var chiTiet = new ChiTietBaiLam
                {
                    CauHoiId = traLoi.CauHoiId,
                    DapAnId = traLoi.DapAnId,
                    GiaTriNhap = traLoi.GiaTriNhap,
                    ChiTietTraLoiMenhDes = new List<ChiTietTraLoiMenhDe>()
                };

                bool isCorrect = false;

                if (traLoi.DapAnId.HasValue)
                {
                    var dapAnDung = cauHoiGoc.DapAns.FirstOrDefault(d => d.LaDapAnDung);
                    if (dapAnDung != null && dapAnDung.Id == traLoi.DapAnId) isCorrect = true;
                }
                else if (traLoi.GiaTriNhap.HasValue)
                {
                    var dapAnDien = cauHoiGoc.DapAnDienKetQuas.FirstOrDefault();
                    if (dapAnDien != null)
                    {
                        float min = dapAnDien.GiaTriDung - dapAnDien.SaiSoChoPhep;
                        float max = dapAnDien.GiaTriDung + dapAnDien.SaiSoChoPhep;
                        if (traLoi.GiaTriNhap.Value >= min && traLoi.GiaTriNhap.Value <= max) isCorrect = true;
                    }
                }
                else if (traLoi.MenhDes.Any())
                {
                    int soYChinhXac = 0;
                    foreach (var md in traLoi.MenhDes)
                    {
                        var mdGoc = cauHoiGoc.MenhDeDungSais.FirstOrDefault(m => m.Id == md.MenhDeId);
                        if (mdGoc != null && mdGoc.LaDung == md.LuaChonCuaHocVien) soYChinhXac++;

                        chiTiet.ChiTietTraLoiMenhDes.Add(new ChiTietTraLoiMenhDe
                        {
                            MenhDeDungSaiId = md.MenhDeId,
                            LuaChonCuaHocVien = md.LuaChonCuaHocVien
                        });
                    }
                    if (soYChinhXac == cauHoiGoc.MenhDeDungSais.Count) isCorrect = true;
                }

                if (isCorrect) soCauDung++;
                baiLam.ChiTietBaiLams.Add(chiTiet);
            }

            baiLam.SoCauDung = soCauDung;
            baiLam.Diem = tongSoCau > 0 ? (float)Math.Round(((float)soCauDung / tongSoCau) * 10, 2) : 0;

            await _unitOfWork.BaiLamRepository.AddAsync(baiLam);
            await _unitOfWork.CompleteAsync(userId);

            return baiLam.Diem;
        }
    }
}

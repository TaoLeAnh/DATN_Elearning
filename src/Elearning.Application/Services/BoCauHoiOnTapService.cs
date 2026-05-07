using Elearning.Application.Interfaces;
using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Services
{
    public class BoCauHoiOnTapService : IBoCauHoiOnTapService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _RequestContext;

        public BoCauHoiOnTapService(IUnitOfWork unitOfWork, IRequestContext requestContext)
        {
            _unitOfWork = unitOfWork;
            _RequestContext = requestContext;
        }

        public async Task<DataTableJson> GetPaged(BoCauHoiOnTapQuery searchOption)
        {
            var (items, total) = await _unitOfWork.BoCauHoiOnTapRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        //public async Task<BoCauHoiOnTapDto> GetByIdAsync(Guid id)
        //{
        //    var entity = await _unitOfWork.BoCauHoiOnTapRepository.FindAsync(
        //        x => x.Id == id,
        //        // Bổ sung "KhoaHoc", "ChuongHoc", "BaiHoc" vào mảng Include
        //        includes: new[] { "GiangVien", "KhoaHoc", "ChuongHoc", "BaiHoc", "ChiTietBoCauHois", "ChiTietBoCauHois.CauHoi" }
        //    );

        //    if (entity == null) throw new ArgumentException("Không tìm thấy bộ câu hỏi.");

        //    return new BoCauHoiOnTapDto
        //    {
        //        Id = entity.Id,
        //        TenBo = entity.TenBo,
        //        MoTa = entity.MoTa,
        //        LoaiBoCauHoi = entity.LoaiBoCauHoi,

        //        KhoaHocId = entity.KhoaHocId,
        //        TenKhoaHoc = entity.KhoaHoc?.TenKhoaHoc, // Lấy tên Khóa

        //        ChuongHocId = entity.ChuongHocId,
        //        TenChuongHoc = entity.ChuongHoc?.TenChuong, // Lấy tên Chương

        //        BaiHocId = entity.BaiHocId,
        //        TenBaiHoc = entity.BaiHoc?.TieuDe, // Lấy tên Bài Học

        //        GiangVienId = entity.GiangVienId,
        //        TenGiangVien = entity.GiangVien?.Ten,
        //        Created = entity.Created,
        //        LastModified = entity.LastModified,
        //        ChiTietBoCauHois = entity.ChiTietBoCauHois.Select(c => new ChiTietBoCauHoiDto
        //        {
        //            Id = c.Id,
        //            BoCauHoiOnTapId = c.BoCauHoiOnTapId,
        //            CauHoiId = c.CauHoiId,
        //            ThuTu = c.ThuTu,
        //            NoiDungCauHoi = c.CauHoi?.NoiDung
        //        }).OrderBy(c => c.ThuTu).ToList()
        //    };
        //}
        public async Task<BoCauHoiOnTapDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.BoCauHoiOnTapRepository.FindAsync(
                x => x.Id == id,
                // ĐÃ SỬA: Thêm Include sâu vào 3 bảng đáp án của câu hỏi
                includes: new[] {
                    "GiangVien", "KhoaHoc", "ChuongHoc", "BaiHoc",
                    "ChiTietBoCauHois", "ChiTietBoCauHois.CauHoi",
                    "ChiTietBoCauHois.CauHoi.DapAns",
                    "ChiTietBoCauHois.CauHoi.MenhDeDungSais",
                    "ChiTietBoCauHois.CauHoi.DapAnDienKetQuas"
                }
            );

            if (entity == null) throw new ArgumentException("Không tìm thấy bộ câu hỏi.");

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
                Created = entity.Created,
                LastModified = entity.LastModified,

                // ĐÃ SỬA: Map toàn bộ 3 loại đáp án ra DTO tương ứng
                ChiTietBoCauHois = entity.ChiTietBoCauHois.Select(c => new ChiTietBoCauHoiDto
                {
                    Id = c.Id,
                    BoCauHoiOnTapId = c.BoCauHoiOnTapId,
                    CauHoiId = c.CauHoiId,
                    ThuTu = c.ThuTu,
                    NoiDungCauHoi = c.CauHoi?.NoiDung,
                    LoaiCauHoi = c.CauHoi?.LoaiCauHoi ?? EnumLoaiCauHoi.MotLuaChon,

                    DapAns = c.CauHoi?.DapAns?.Select(d => new DapAnDto { Id = d.Id, NoiDung = d.NoiDung, ThuTu = d.ThuTu }).OrderBy(d => d.ThuTu).ToList() ?? new(),

                    MenhDeDungSais = c.CauHoi?.MenhDeDungSais?.Select(m => new MenhDeDungSaiDto { Id = m.Id, NoiDung = m.NoiDung, ThuTu = m.ThuTu }).OrderBy(m => m.ThuTu).ToList() ?? new(),

                    DapAnDienKetQuas = c.CauHoi?.DapAnDienKetQuas?.Select(dk => new DapAnDienKetQuaDto { Id = dk.Id }).ToList() ?? new()
                }).OrderBy(c => c.ThuTu).ToList()
            };
        }
        public async Task<Guid> CreateAsync(BoCauHoiOnTapForm form)
        {
            var entity = new BoCauHoiOnTap
            {
                TenBo = form.TenBo,
                MoTa = form.MoTa,
                LoaiBoCauHoi = form.LoaiBoCauHoi,
                ThoiLuongPhut = form.ThoiLuongPhut,
                BaiHocId = form.BaiHocId,
                ChuongHocId = form.ChuongHocId,
                KhoaHocId = form.KhoaHocId,
                GiangVienId = form.GiangVienId,
                ChiTietBoCauHois = form.ChiTietBoCauHois.Select(c => new ChiTietBoCauHoi
                {
                    CauHoiId = c.CauHoiId,
                    ThuTu = c.ThuTu
                }).ToList()
            };

            await _unitOfWork.BoCauHoiOnTapRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, BoCauHoiOnTapForm item)
        {
            var itemUpdate = await _unitOfWork.BoCauHoiOnTapRepository.FindAsync(
                x => x.Id == id,
                includes: new[] { "ChiTietBoCauHois" }
            );
            if (itemUpdate == null) return false;

            itemUpdate.TenBo = item.TenBo;
            itemUpdate.MoTa = item.MoTa;
            itemUpdate.LoaiBoCauHoi = item.LoaiBoCauHoi;
            itemUpdate.ThoiLuongPhut = item.ThoiLuongPhut;
            itemUpdate.BaiHocId = item.BaiHocId;
            itemUpdate.ChuongHocId = item.ChuongHocId;
            itemUpdate.KhoaHocId = item.KhoaHocId;
            itemUpdate.GiangVienId = item.GiangVienId;

            var existingIds = itemUpdate.ChiTietBoCauHois.Select(c => c.CauHoiId).ToList();
            var newIds = item.ChiTietBoCauHois.Select(c => c.CauHoiId).ToList();

            // Xóa những cái không còn trong danh sách mới
            var toDelete = itemUpdate.ChiTietBoCauHois
                .Where(c => !newIds.Contains(c.CauHoiId))
                .ToList();

            if (toDelete.Any())
                _unitOfWork.ChiTietBoCauHoiRepository.DeleteRange(toDelete);

            // Thêm những cái mới chưa có
            var toAdd = item.ChiTietBoCauHois
                .Where(c => !existingIds.Contains(c.CauHoiId))
                .ToList();

            foreach (var a in toAdd)
                itemUpdate.ChiTietBoCauHois.Add(new ChiTietBoCauHoi
                {
                    CauHoiId = a.CauHoiId,
                    ThuTu = a.ThuTu,
                    BoCauHoiOnTapId = itemUpdate.Id
                });

            // Cập nhật ThuTu cho các item đã tồn tại
            foreach (var existing in itemUpdate.ChiTietBoCauHois
                .Where(c => newIds.Contains(c.CauHoiId)).ToList())
            {
                var match = item.ChiTietBoCauHois.FirstOrDefault(c => c.CauHoiId == existing.CauHoiId);
                if (match != null) existing.ThuTu = match.ThuTu;
            }

            _unitOfWork.BoCauHoiOnTapRepository.Update(itemUpdate);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var itemDelete = await _unitOfWork.BoCauHoiOnTapRepository.GetByIdAsync(id);
            if (itemDelete == null) return false;

            _unitOfWork.BoCauHoiOnTapRepository.Delete(itemDelete);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }
    }
}

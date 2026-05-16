using Elearning.Application.Interfaces;
using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elearning.Application.Services
{
    public class CauHoiService : ICauHoiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _RequestContext;
        private readonly IStorageService _storageService;

        public CauHoiService(IUnitOfWork unitOfWork, IRequestContext requestContext, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _RequestContext = requestContext;
            _storageService = storageService;
        }

        public async Task<DataTableJson> GetPaged(CauHoiQuery searchOption)
        {
            var (items, total) = await _unitOfWork.CauHoiRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<CauHoiDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.CauHoiRepository.FindAsync(
                x => x.Id == id,
                // 👉 BỔ SUNG INCLUDE ĐẦY ĐỦ 3 BẢNG
                includes: new[] { "DapAns", "MenhDeDungSais", "DapAnDienKetQuas", "GiangVien" }
            );

            if (entity == null) throw new ArgumentException("Không tìm thấy câu hỏi.");

            return new CauHoiDto
            {
                Id = entity.Id,
                NoiDung = entity.NoiDung,
                HinhAnhUrl = entity.HinhAnhUrl,
                KhoaHocId = entity.KhoaHocId,
                MonHoc = entity.MonHoc,
                LoaiCauHoi = entity.LoaiCauHoi,
                MucDo = entity.MucDo,
                ChuDe = entity.ChuDe,
                GiaiThich = entity.GiaiThich,
                GiangVienId = entity.GiangVienId,
                TenGiangVien = entity.GiangVien?.Ten,
                Created = entity.Created,
                LastModified = entity.LastModified,

                // 1. Trắc nghiệm
                DapAns = entity.DapAns?.Select(d => new DapAnDto
                {
                    Id = d.Id,
                    CauHoiId = d.CauHoiId,
                    NoiDung = d.NoiDung,
                    HinhAnhUrl = d.HinhAnhUrl,
                    LaDapAnDung = d.LaDapAnDung,
                    ThuTu = d.ThuTu
                }).OrderBy(d => d.ThuTu).ToList() ?? new(),

                // 👉 2. BỔ SUNG MAP: Mệnh đề Đúng/Sai
                MenhDeDungSais = entity.MenhDeDungSais?.Select(m => new MenhDeDungSaiDto
                {
                    Id = m.Id,
                    NoiDung = m.NoiDung,
                    HinhAnhUrl = m.HinhAnhUrl,
                    LaDung = m.LaDung,
                    ThuTu = m.ThuTu
                }).OrderBy(m => m.ThuTu).ToList() ?? new(),

                // 👉 3. BỔ SUNG MAP: Điền kết quả
                DapAnDienKetQuas = entity.DapAnDienKetQuas?.Select(dk => new DapAnDienKetQuaDto
                {
                    Id = dk.Id,
                    GiaTriDung = dk.GiaTriDung,
                    SaiSoChoPhep = dk.SaiSoChoPhep
                }).ToList() ?? new()
            };
        }

        public async Task<Guid> CreateAsync(CauHoiForm form)
        {
            var entity = new CauHoi
            {
                NoiDung = form.NoiDung,
                KhoaHocId = form.KhoaHocId,
                MonHoc = form.MonHoc,
                HinhAnhUrl = form.HinhAnhUrl,
                LoaiCauHoi = form.LoaiCauHoi,
                MucDo = form.MucDo,
                ChuDe = form.ChuDe,
                GiaiThich = form.GiaiThich,
                GiangVienId = form.GiangVienId,

                // Xử lý lưu loại 1
                DapAns = form.LoaiCauHoi == EnumLoaiCauHoi.MotLuaChon ? form.DapAns.Select(d => new DapAn
                {
                    NoiDung = d.NoiDung,
                    HinhAnhUrl = d.HinhAnhUrl,
                    LaDapAnDung = d.LaDapAnDung,
                    ThuTu = d.ThuTu
                }).ToList() : new List<DapAn>(),

                // 👉 Xử lý lưu loại 2
                MenhDeDungSais = form.LoaiCauHoi == EnumLoaiCauHoi.MenhDeDungSai ? form.MenhDeDungSais.Select(m => new MenhDeDungSai
                {
                    NoiDung = m.NoiDung,
                    HinhAnhUrl = m.HinhAnhUrl,
                    LaDung = m.LaDung,
                    ThuTu = m.ThuTu
                }).ToList() : new List<MenhDeDungSai>(),

                // 👉 Xử lý lưu loại 3
                DapAnDienKetQuas = form.LoaiCauHoi == EnumLoaiCauHoi.DienKetQua ? form.DapAnDienKetQuas.Select(dk => new DapAnDienKetQua
                {
                    GiaTriDung = dk.GiaTriDung,
                    SaiSoChoPhep = dk.SaiSoChoPhep
                }).ToList() : new List<DapAnDienKetQua>()
            };

            await _unitOfWork.CauHoiRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, CauHoiForm item)
        {
            // 👉 BỔ SUNG INCLUDE
            var itemUpdate = await _unitOfWork.CauHoiRepository.FindAsync(x => x.Id == id, includes: new[] { "DapAns", "MenhDeDungSais", "DapAnDienKetQuas" });
            if (itemUpdate == null) return false;

            // ... Giữ nguyên phần xử lý xóa file MinIO của bác ...

            // Cập nhật thông tin Entity chính
            itemUpdate.NoiDung = item.NoiDung;
            itemUpdate.KhoaHocId = item.KhoaHocId;
            itemUpdate.MonHoc = item.MonHoc;
            itemUpdate.HinhAnhUrl = item.HinhAnhUrl;
            itemUpdate.LoaiCauHoi = item.LoaiCauHoi;
            itemUpdate.MucDo = item.MucDo;
            itemUpdate.ChuDe = item.ChuDe;
            itemUpdate.GiaiThich = item.GiaiThich;
            itemUpdate.GiangVienId = item.GiangVienId;

            // XÓA TẤT CẢ CÁC ĐÁP ÁN CŨ ĐỂ LÀM SẠCH DB
            foreach (var oldDapAn in itemUpdate.DapAns.ToList()) _unitOfWork.DapAnRepository.Delete(oldDapAn);
            // (Bác có Repository của MenhDe và DienKetQua thì gọi .Delete() giống thế này, 
            // hoặc do bác đã cấu hình Cascade Delete ở Entity rồi thì Clear() list là EF Core tự xóa)
            itemUpdate.DapAns.Clear();
            itemUpdate.MenhDeDungSais.Clear();
            itemUpdate.DapAnDienKetQuas.Clear();

            // THÊM ĐÁP ÁN MỚI TÙY THEO LOẠI CÂU HỎI
            if (item.LoaiCauHoi == EnumLoaiCauHoi.MotLuaChon)
            {
                foreach (var d in item.DapAns) itemUpdate.DapAns.Add(new DapAn { NoiDung = d.NoiDung, HinhAnhUrl = d.HinhAnhUrl, LaDapAnDung = d.LaDapAnDung, ThuTu = d.ThuTu });
            }
            else if (item.LoaiCauHoi == EnumLoaiCauHoi.MenhDeDungSai)
            {
                foreach (var m in item.MenhDeDungSais) itemUpdate.MenhDeDungSais.Add(new MenhDeDungSai { NoiDung = m.NoiDung, HinhAnhUrl = m.HinhAnhUrl, LaDung = m.LaDung, ThuTu = m.ThuTu });
            }
            else if (item.LoaiCauHoi == EnumLoaiCauHoi.DienKetQua)
            {
                foreach (var dk in item.DapAnDienKetQuas) itemUpdate.DapAnDienKetQuas.Add(new DapAnDienKetQua { GiaTriDung = dk.GiaTriDung, SaiSoChoPhep = dk.SaiSoChoPhep });
            }

            _unitOfWork.CauHoiRepository.Update(itemUpdate);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var itemDelete = await _unitOfWork.CauHoiRepository.GetByIdAsync(id);
            if (itemDelete == null) return false;
            if (!string.IsNullOrEmpty(itemDelete.HinhAnhUrl))
            {
                await _storageService.DeleteFileAsync(itemDelete.HinhAnhUrl);
            }

            _unitOfWork.CauHoiRepository.Delete(itemDelete);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }

        public async Task<List<string>> GetDanhSachChuDeTheoKyThiAsync(Guid kyThiId)
        {
            var kyThi = await _unitOfWork.KyThiRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(x => x.Id == kyThiId);

            if (kyThi == null)
            {
                throw new ArgumentException("Không tìm thấy thông tin Kỳ thi.");
            }

            // ĐÃ SỬA LỖI LOGIC TÌM CHỦ ĐỀ CHUẨN XÁC HƠN
            // Tự động phân nhánh: Đề public thì tìm theo Môn học, Đề nội bộ thì tìm theo Khóa học
            var query = _unitOfWork.CauHoiRepository.GetTableNoTracking();

            if (kyThi.IsPublic && kyThi.MonHoc.HasValue)
            {
                query = query.Where(x => x.MonHoc == kyThi.MonHoc);
            }
            else if (!kyThi.IsPublic && kyThi.KhoaHocId.HasValue)
            {
                query = query.Where(x => x.KhoaHocId == kyThi.KhoaHocId);
            }
            else
            {
                return new List<string>(); // Không hợp lệ thì trả về rỗng
            }

            var danhSachChuDe = await query
                .Where(x => !string.IsNullOrEmpty(x.ChuDe))
                .Select(x => x.ChuDe.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            return danhSachChuDe;
        }
    }
}
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
                includes: new[] { "DapAns", "GiangVien" }
            );

            if (entity == null) throw new ArgumentException("Không tìm thấy câu hỏi.");

            return new CauHoiDto
            {
                Id = entity.Id,
                NoiDung = entity.NoiDung,
                HinhAnhUrl = entity.HinhAnhUrl,
                KhoaHocId = entity.KhoaHocId,
                MonHoc = entity.MonHoc, // BỔ SUNG MAP OUT
                LoaiCauHoi = entity.LoaiCauHoi,
                MucDo = entity.MucDo,
                ChuDe = entity.ChuDe,
                GiaiThich = entity.GiaiThich,
                GiangVienId = entity.GiangVienId,
                TenGiangVien = entity.GiangVien?.Ten,
                Created = entity.Created,
                LastModified = entity.LastModified,
                DapAns = entity.DapAns.Select(d => new DapAnDto
                {
                    Id = d.Id,
                    CauHoiId = d.CauHoiId,
                    NoiDung = d.NoiDung,
                    HinhAnhUrl = d.HinhAnhUrl,
                    LaDapAnDung = d.LaDapAnDung,
                    ThuTu = d.ThuTu
                }).OrderBy(d => d.ThuTu).ToList()
            };
        }

        public async Task<Guid> CreateAsync(CauHoiForm form)
        {
            var entity = new CauHoi
            {
                NoiDung = form.NoiDung,
                KhoaHocId = form.KhoaHocId,
                MonHoc = form.MonHoc, // BỔ SUNG MAP IN
                HinhAnhUrl = form.HinhAnhUrl,
                LoaiCauHoi = form.LoaiCauHoi,
                MucDo = form.MucDo,
                ChuDe = form.ChuDe,
                GiaiThich = form.GiaiThich,
                GiangVienId = form.GiangVienId,

                DapAns = form.DapAns.Select(d => new DapAn
                {
                    NoiDung = d.NoiDung,
                    HinhAnhUrl = d.HinhAnhUrl,
                    LaDapAnDung = d.LaDapAnDung,
                    ThuTu = d.ThuTu
                }).ToList()
            };

            await _unitOfWork.CauHoiRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, CauHoiForm item)
        {
            var itemUpdate = await _unitOfWork.CauHoiRepository.FindAsync(x => x.Id == id, includes: new[] { "DapAns" });
            if (itemUpdate == null) return false;

            // Xử lý File trên MinIO (Giữ nguyên)
            var newImageUrls = new List<string>();
            if (!string.IsNullOrEmpty(item.HinhAnhUrl)) newImageUrls.Add(item.HinhAnhUrl);
            foreach (var d in item.DapAns.Where(x => !string.IsNullOrEmpty(x.HinhAnhUrl)))
            {
                newImageUrls.Add(d.HinhAnhUrl);
            }

            var oldImageUrls = new List<string>();
            if (!string.IsNullOrEmpty(itemUpdate.HinhAnhUrl)) oldImageUrls.Add(itemUpdate.HinhAnhUrl);
            foreach (var d in itemUpdate.DapAns.Where(x => !string.IsNullOrEmpty(x.HinhAnhUrl)))
            {
                oldImageUrls.Add(d.HinhAnhUrl);
            }

            var imagesToDelete = oldImageUrls.Except(newImageUrls).ToList();
            foreach (var url in imagesToDelete)
            {
                await _storageService.DeleteFileAsync(url);
            }

            // Cập nhật thông tin Entity
            itemUpdate.NoiDung = item.NoiDung;
            itemUpdate.KhoaHocId = item.KhoaHocId;
            itemUpdate.MonHoc = item.MonHoc; // BỔ SUNG MAP UPDATE
            itemUpdate.HinhAnhUrl = item.HinhAnhUrl;
            itemUpdate.LoaiCauHoi = item.LoaiCauHoi;
            itemUpdate.MucDo = item.MucDo;
            itemUpdate.ChuDe = item.ChuDe;
            itemUpdate.GiaiThich = item.GiaiThich;
            itemUpdate.GiangVienId = item.GiangVienId;

            // Xử lý đáp án (Giữ nguyên)
            var oldDapAns = itemUpdate.DapAns.ToList();
            foreach (var oldDapAn in oldDapAns)
            {
                _unitOfWork.DapAnRepository.Delete(oldDapAn);
            }

            itemUpdate.DapAns.Clear();

            foreach (var d in item.DapAns)
            {
                itemUpdate.DapAns.Add(new DapAn
                {
                    NoiDung = d.NoiDung,
                    HinhAnhUrl = d.HinhAnhUrl,
                    LaDapAnDung = d.LaDapAnDung,
                    ThuTu = d.ThuTu,
                    CauHoiId = itemUpdate.Id
                });
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
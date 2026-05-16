using Elearning.Application.Interfaces;
using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Services
{
    public class BaiHocService : IBaiHocService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _RequestContext;
        private readonly VideoTranscriptQueue _queue;

        public BaiHocService(IUnitOfWork unitOfWork, IRequestContext requestContext, VideoTranscriptQueue queue)
        {
            _unitOfWork = unitOfWork;
            _RequestContext = requestContext;
            _queue = queue;
        }

        public async Task<DataTableJson> GetPaged(BaiHocQuery searchOption)
        {
            var (items, total) = await _unitOfWork.BaiHocRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<BaiHocDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.BaiHocRepository.FindAsync(
                x => x.Id == id,
                includes: new[] { "ChuongHoc" }
            );

            if (entity == null) throw new ArgumentException("Không tìm thấy bài học.");

            return new BaiHocDto
            {
                Id = entity.Id,
                TieuDe = entity.TieuDe,
                NoiDung = entity.NoiDung,
                VideoUrl = entity.VideoUrl,
                ThoiLuong = entity.ThoiLuong,
                Loai = entity.Loai,
                ChuongHocId = entity.ChuongHocId,
                // Lấy tên chương học từ bảng đã Join
                TenChuong = entity.ChuongHoc?.TenChuong,
                ThuTu = entity.ThuTu,
                Created = entity.Created,
                LastModified = entity.LastModified
            };
        }

        public async Task<Guid> CreateAsync(BaiHocForm form)
        {
            var entity = new BaiHoc
            {
                TieuDe = form.TieuDe,
                NoiDung = form.NoiDung,
                VideoUrl = form.VideoUrl,
                ThoiLuong = form.ThoiLuong,
                Loai = form.Loai,
                ChuongHocId = form.ChuongHocId,
                ThuTu = form.ThuTu
            };

            await _unitOfWork.BaiHocRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);

            // 👉 ĐÃ FIX: Dùng biến entity và check cả 2 loại link
            if (!string.IsNullOrEmpty(entity.VideoUrl) &&
               (entity.VideoUrl.Contains("youtube.com") || entity.VideoUrl.Contains("youtu.be")))
            {
                await _queue.QueueWorkItemAsync(new TranscriptWorkItem
                {
                    BaiHocId = entity.Id,
                    VideoUrl = entity.VideoUrl
                });
            }

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, BaiHocForm item)
        {
            var itemUpdate = await _unitOfWork.BaiHocRepository.GetByIdAsync(id);
            if (itemUpdate == null) return false;

            bool isVideoChanged = itemUpdate.VideoUrl != item.VideoUrl;

            itemUpdate.TieuDe = item.TieuDe;
            itemUpdate.NoiDung = item.NoiDung;
            itemUpdate.VideoUrl = item.VideoUrl;
            itemUpdate.ThoiLuong = item.ThoiLuong;
            itemUpdate.Loai = item.Loai;
            itemUpdate.ChuongHocId = item.ChuongHocId;
            itemUpdate.ThuTu = item.ThuTu;

            _unitOfWork.BaiHocRepository.Update(itemUpdate);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);

            // 👉 ĐÃ FIX: Thêm điều kiện check link rút gọn youtu.be
            if (isVideoChanged && !string.IsNullOrEmpty(itemUpdate.VideoUrl) &&
               (itemUpdate.VideoUrl.Contains("youtube.com") || itemUpdate.VideoUrl.Contains("youtu.be")))
            {
                await _queue.QueueWorkItemAsync(new TranscriptWorkItem
                {
                    BaiHocId = itemUpdate.Id,
                    VideoUrl = itemUpdate.VideoUrl
                });
            }

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var itemDelete = await _unitOfWork.BaiHocRepository.GetByIdAsync(id);
            if (itemDelete == null) return false;

            _unitOfWork.BaiHocRepository.Delete(itemDelete);
            await _unitOfWork.CompleteAsync(_RequestContext.CurrentIdUser);
            return true;
        }
    }
}

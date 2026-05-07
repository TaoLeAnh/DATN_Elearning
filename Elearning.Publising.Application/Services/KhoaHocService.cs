using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Infrastructure.Repository.UnitOfWorks;
using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Services
{
    public class KhoaHocService : IKhoaHocService
    {
        private readonly IUnitOfWorkPublising _UnitOfWork;
        public KhoaHocService(IUnitOfWorkPublising UnitOfWork) { _UnitOfWork = UnitOfWork; }

        public async Task<DataTableJson> GetPaged(KhoaHocQuery searchOption)
        {
            var (items, total) = await _UnitOfWork.KhoaHocRepository.GetPagedDtoAsync(searchOption);
            return new DataTableJson(items, searchOption.draw, total, items.Count);
        }

        public async Task<KhoaHocDto> GetByIdAsync(Guid id)
        {
            KhoaHoc entity = await _UnitOfWork.KhoaHocRepository.FindAsync(x => x.Id == id, new[] { "GiangVien" });

            if (entity == null) return null;

            return MapToDto(entity);
        }

        public async Task<KhoaHocDto> GetBySlugAsync(string slug)
        {
            // TẠM THỜI RETURN NULL VÌ DATABASE CHƯA CÓ CỘT SLUG
            // Sau này bạn thêm cột Slug vào Entity KhoaHoc thì mở comment dòng dưới ra nhé:
            // KhoaHoc entity = await _UnitOfWork.KhoaHocRepository.FindAsync(x => x.Slug == slug, new[] { "GiangVien" });
            // if (entity == null) return null;
            // return MapToDto(entity);

            await Task.CompletedTask; // Giữ cho hàm async không bị cảnh báo
            return null;
        }

        // Hàm hỗ trợ Map chung để code không bị lặp lại
        private KhoaHocDto MapToDto(KhoaHoc entity)
        {
            return new KhoaHocDto
            {
                Id = entity.Id,
                TenKhoaHoc = entity.TenKhoaHoc,
                MoTa = entity.MoTa,
                GiangVienId = entity.GiangVienId,
                MonHoc = entity.MonHoc,
                TenGiangVien = entity.GiangVien.Ten,
                Created = entity.Created,
                LastModified = entity.LastModified
            };
        }
        public async Task<KhoaHocDto> GetDetailByIdAsync(Guid id)
        {
            // 1. Lấy thông tin Khóa học, Chương, Bài (Như cũ)
            var query = _UnitOfWork.KhoaHocRepository.GetTableNoTracking()
                .Include(x => x.GiangVien)
                .Include(x => x.ChuongHocs)
                    .ThenInclude(c => c.BaiHocs);

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return null;

            // 2. TÌM TẤT CẢ BỘ CÂU HỎI LIÊN QUAN ĐẾN KHÓA HỌC NÀY
            // Dùng repository để query bảng BoCauHoiOnTap
            var lstBoCauHoi = await _UnitOfWork.BoCauHoiOnTapRepository.GetTableNoTracking()
                .Where(x => x.KhoaHocId == id)
                .Select(x => new BoCauHoiOnTapDto
                {
                    Id = x.Id,
                    TenBo = x.TenBo,
                    LoaiBoCauHoi = x.LoaiBoCauHoi,
                    KhoaHocId = x.KhoaHocId,
                    ChuongHocId = x.ChuongHocId,
                    BaiHocId = x.BaiHocId
                }).ToListAsync();

            // 3. Map dữ liệu sang DTO
            var dto = new KhoaHocDto
            {
                Id = entity.Id,
                TenKhoaHoc = entity.TenKhoaHoc,
                MoTa = entity.MoTa,
                MonHoc = entity.MonHoc,
                GiangVienId = entity.GiangVienId,
                TenGiangVien = entity.GiangVien?.Ten,
                HinhAnhUrl = entity.HinhAnhUrl,
                GiaGoc = entity.GiaGoc,
                GiaBan = entity.GiaBan,

                // Gán bộ câu hỏi TỔNG (chỉ có KhoaHocId, không thuộc chương/bài cụ thể)
                DanhSachBoCauHoi = lstBoCauHoi.Where(q => q.ChuongHocId == null && q.BaiHocId == null).ToList(),

                // Map Chương Học & Bài Học
                ChuongHocs = entity.ChuongHocs.OrderBy(c => c.ThuTu).Select(c => new ChuongHocDto
                {
                    Id = c.Id,
                    TenChuong = c.TenChuong,
                    ThuTu = c.ThuTu,

                    // Gán bộ câu hỏi CHƯƠNG (có KhoaHocId, ChuongHocId nhưng không có BaiHocId)
                    DanhSachBoCauHoi = lstBoCauHoi.Where(q => q.ChuongHocId == c.Id && q.BaiHocId == null).ToList(),

                    BaiHocs = c.BaiHocs.OrderBy(b => b.ThuTu).Select(b => new BaiHocDto
                    {
                        Id = b.Id,
                        TieuDe = b.TieuDe,
                        ThoiLuong = b.ThoiLuong,
                        ThuTu = b.ThuTu,
                        Loai = b.Loai,
                        VideoUrl = b.VideoUrl,
                        NoiDung = b.NoiDung,

                        // Gán bộ câu hỏi BÀI HỌC (có BaiHocId khớp)
                        DanhSachBoCauHoi = lstBoCauHoi.Where(q => q.BaiHocId == b.Id).ToList()

                    }).ToList()
                }).ToList()
            };

            return dto;
        }
    }
}

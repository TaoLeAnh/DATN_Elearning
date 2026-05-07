using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class KyThiConfiguration : IEntityTypeConfiguration<KyThi>
    {
        public void Configure(EntityTypeBuilder<KyThi> builder)
        {
            builder.ToTable("KyThi");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenKyThi)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ThoiLuongPhut)
                .IsRequired();

            // ====================================================
            // 1. BỔ SUNG CẤU HÌNH CHO CÁC TRƯỜNG THỜI GIAN VÀ PHÂN LOẠI
            // ====================================================

            builder.Property(x => x.ThoiGianBatDau).IsRequired(false);
            builder.Property(x => x.ThoiGianKetThuc).IsRequired(false);

            builder.Property(x => x.MonHoc).IsRequired(false);
            builder.Property(x => x.LoaiDeThi).IsRequired(false);
            builder.Property(x => x.NamThi).IsRequired(false);

            builder.Property(x => x.TinhThanh)
                .HasMaxLength(100)
                .IsRequired(false); // Giới hạn chuỗi tên tỉnh thành cho nhẹ DB

            builder.Property(x => x.TenTruong)
                .HasMaxLength(255)
                .IsRequired(false); // Giới hạn chuỗi tên trường

            builder.Property(x => x.IsPublic)
                .HasDefaultValue(true); // Đặt mặc định tạo ra là đề Public

            // ====================================================
            // 2. CẤU HÌNH LẠI KHÓA NGOẠI (Cho phép Null)
            // ====================================================

            builder.HasOne(x => x.KhoaHoc)
                .WithMany(x => x.KyThis)
                .HasForeignKey(x => x.KhoaHocId)
                .IsRequired(false) // Bắt buộc thêm dòng này để EF Core hiểu đây là quan hệ không bắt buộc
                .OnDelete(DeleteBehavior.SetNull); // Nếu Khóa học bị xóa, Đề thi không bị xóa theo mà chỉ gán KhoaHocId = null
        }
    }
}

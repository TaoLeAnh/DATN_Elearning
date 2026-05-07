using Elearning.Domain.Entities;
using Elearning.Shared.Contracts.Portal.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class DangKyKhoaHocConfiguration : IEntityTypeConfiguration<DangKyKhoaHoc>
    {
        public void Configure(EntityTypeBuilder<DangKyKhoaHoc> builder)
        {
            builder.ToTable("DangKyKhoaHoc");

            builder.HasKey(x => x.Id);

            // Ràng buộc Unique: Một người dùng chỉ đăng ký một khóa học một lần duy nhất
            builder.HasIndex(x => new { x.NguoiDungId, x.KhoaHocId })
                   .IsUnique()
                   .HasDatabaseName("IX_Unique_NguoiDung_KhoaHoc");

            builder.Property(x => x.NgayDangKy)
                   .IsRequired();

            builder.Property(x => x.TrangThai)
                   .IsRequired()
                   .HasDefaultValue(EnumTrangThaiDangKy.ChoDuyet);

            builder.Property(x => x.TienDo)
                   .HasDefaultValue(0.0);

            builder.HasOne(x => x.NguoiDung)
                .WithMany(x => x.DangKyKhoaHocs)
                .HasForeignKey(x => x.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa người dùng làm mất log đăng ký quan trọng

            builder.HasOne(x => x.KhoaHoc)
                .WithMany(x => x.DangKyKhoaHocs)
                .HasForeignKey(x => x.KhoaHocId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

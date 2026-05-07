using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class HoSoGiaoVienConfiguration : IEntityTypeConfiguration<HoSoGiaoVien>
    {
        public void Configure(EntityTypeBuilder<HoSoGiaoVien> builder)
        {
            builder.ToTable("HoSoGiaoVien");

            builder.HasKey(x => x.Id);

            // Giới hạn độ dài các trường chuỗi
            builder.Property(x => x.AnhDaiDienUrl)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(x => x.ThanhTichNoiBat)
                .HasMaxLength(2000) // Cho phép nhập text dài
                .IsRequired(false);

            builder.Property(x => x.PhuongPhapGiangDay)
                .HasMaxLength(2000) // Cho phép nhập text dài
                .IsRequired(false);

            builder.Property(x => x.MonHocChuyenMon)
                .IsRequired();

            // CẤU HÌNH QUAN HỆ 1-1 (Rất quan trọng)
            builder.HasOne(x => x.NguoiDung)
                .WithOne(x => x.HoSoGiaoVien)
                .HasForeignKey<HoSoGiaoVien>(x => x.NguoiDungId)
                .OnDelete(DeleteBehavior.Cascade); // Nếu xóa NguoiDung, HoSoGiaoVien sẽ tự động bị xóa theo
        }
    }
}

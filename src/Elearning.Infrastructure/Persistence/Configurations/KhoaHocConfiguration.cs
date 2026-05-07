using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class KhoaHocConfiguration : IEntityTypeConfiguration<KhoaHoc>
    {
        public void Configure(EntityTypeBuilder<KhoaHoc> builder)
        {
            builder.ToTable("KhoaHoc");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenKhoaHoc)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.MoTa)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.MonHoc)
                .IsRequired();

            builder.Property(x => x.HinhAnhUrl)
                .HasMaxLength(1000)   // Giới hạn link ảnh 1000 ký tự
                .IsRequired(false);   // Cho phép null

            builder.Property(x => x.GiaGoc)
                .HasColumnType("decimal(18,0)") // Kiểu tiền tệ không có phần thập phân
                .IsRequired(false);

            builder.Property(x => x.GiaBan)
                .HasColumnType("decimal(18,0)")
                .IsRequired(false);

            builder.HasOne(x => x.GiangVien)
                .WithMany(x => x.KhoaHocGiangDays)
                .HasForeignKey(x => x.GiangVienId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class BoCauHoiOnTapConfiguration : IEntityTypeConfiguration<BoCauHoiOnTap>
    {
        public void Configure(EntityTypeBuilder<BoCauHoiOnTap> builder)
        {
            builder.ToTable("BoCauHoiOnTap");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenBo)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.MoTa)
                .IsRequired(false)
                .HasColumnType("nvarchar(max)");
            builder.Property(x => x.ThoiLuongPhut)
                .IsRequired()
                .HasDefaultValue(45);
            // Khóa ngoại Giảng viên
            builder.HasOne(x => x.GiangVien)
                .WithMany(x => x.BoCauHoiOnTaps)
                .HasForeignKey(x => x.GiangVienId)
                .OnDelete(DeleteBehavior.Restrict);

            // Khóa ngoại Bài học, Chương, Khóa (Cascade để dọn rác tự động)
            builder.HasOne(x => x.BaiHoc)
                .WithMany()
                .HasForeignKey(x => x.BaiHocId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ChuongHoc)
                .WithMany()
                .HasForeignKey(x => x.ChuongHocId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

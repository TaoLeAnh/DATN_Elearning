using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class BaiHocConfiguration : IEntityTypeConfiguration<BaiHoc>
    {
        public void Configure(EntityTypeBuilder<BaiHoc> builder)
        {
            builder.ToTable("BaiHoc");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TieuDe)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.NoiDung)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property(x => x.VideoUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.Loai)
                .IsRequired();

            builder.Property(x => x.ThoiLuong)
                .HasDefaultValue(0);

            builder.Property(x => x.ThuTu)
                .HasDefaultValue(0);


            builder.HasOne(x => x.ChuongHoc)
                .WithMany(x => x.BaiHocs)
                .HasForeignKey(x => x.ChuongHocId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

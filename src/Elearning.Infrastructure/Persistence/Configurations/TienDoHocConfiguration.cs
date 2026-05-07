using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class TienDoHocConfiguration : IEntityTypeConfiguration<TienDoHoc>
    {
        public void Configure(EntityTypeBuilder<TienDoHoc> builder)
        {
            builder.ToTable("TienDoHoc");

            builder.HasKey(x => x.Id);

            // Ràng buộc Unique: Một người dùng chỉ có một bản ghi tiến độ cho một bài học
            builder.HasIndex(x => new { x.NguoiDungId, x.BaiHocId })
                   .IsUnique()
                   .HasDatabaseName("IX_Unique_NguoiDung_BaiHoc");

            builder.Property(x => x.DaHoanThanh)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(x => x.LastTimePosition)
                   .HasDefaultValue(0);

            builder.HasOne(x => x.NguoiDung)
                .WithMany(x => x.TienDoHocs)
                .HasForeignKey(x => x.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BaiHoc)
                .WithMany() // Một bài học có thể xuất hiện trong nhiều bản ghi tiến độ của nhiều người
                .HasForeignKey(x => x.BaiHocId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

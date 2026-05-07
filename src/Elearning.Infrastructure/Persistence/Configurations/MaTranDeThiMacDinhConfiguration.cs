using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class MaTranDeThiMacDinhConfiguration : IEntityTypeConfiguration<MaTranDeThiMacDinh>
    {
        public void Configure(EntityTypeBuilder<MaTranDeThiMacDinh> builder)
        {
            builder.ToTable("MaTranDeThiMacDinh");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenMaTran)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.MonHoc)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(false);

            // Cấu hình quan hệ 1-Nhiều với bảng Chi Tiết
            builder.HasMany(x => x.ChiTiets)
                .WithOne(x => x.MaTran)
                .HasForeignKey(x => x.MaTranDeThiMacDinhId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Ma Trận cha thì tự động xóa các Chi Tiết con
        }
    }
}

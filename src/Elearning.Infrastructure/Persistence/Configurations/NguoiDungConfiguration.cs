using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class NguoiDungConfiguration : IEntityTypeConfiguration<NguoiDung>
    {
        public void Configure(EntityTypeBuilder<NguoiDung> builder)
        {
            builder.ToTable("NguoiDung");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Ten)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.MaHocSinh)
                .HasMaxLength(50) 
                .IsRequired(false);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.MatKhau)
                .IsRequired()
                .HasMaxLength(500);

            // --- SỬA ĐOẠN NÀY ---
            builder.Property(x => x.VaiTro)
                .IsRequired();
            // Xóa .HasMaxLength(50) đi vì Enum lưu dạng số (int) không dùng MaxLength.
        }
    }
}

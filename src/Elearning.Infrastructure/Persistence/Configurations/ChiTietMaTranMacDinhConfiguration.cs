using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class ChiTietMaTranMacDinhConfiguration : IEntityTypeConfiguration<ChiTietMaTranMacDinh>
    {
        public void Configure(EntityTypeBuilder<ChiTietMaTranMacDinh> builder)
        {
            builder.ToTable("ChiTietMaTranMacDinh");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ChuDe)
                .HasMaxLength(300)
                .IsRequired(false); // Có thể null nếu ma trận chỉ cấu hình lấy ngẫu nhiên chung chung

            builder.Property(x => x.SoLuong)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(x => x.PhanThi)
                .IsRequired();

            builder.Property(x => x.LoaiCauHoi)
                .IsRequired();

            builder.Property(x => x.MucDo)
                .IsRequired();

            // Khai báo lại đầu bên này của relation cho chắc cú (tùy chọn)
            builder.HasOne(x => x.MaTran)
                .WithMany(x => x.ChiTiets)
                .HasForeignKey(x => x.MaTranDeThiMacDinhId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

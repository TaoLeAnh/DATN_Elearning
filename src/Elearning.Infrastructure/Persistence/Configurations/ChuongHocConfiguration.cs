using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class ChuongHocConfiguration : IEntityTypeConfiguration<ChuongHoc>
    {
        public void Configure(EntityTypeBuilder<ChuongHoc> builder)
        {
            builder.ToTable("ChuongHoc");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenChuong)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ThuTu)
                .IsRequired();

            builder.HasOne(x => x.KhoaHoc)
                .WithMany(x => x.ChuongHocs)
                .HasForeignKey(x => x.KhoaHocId);
        }
    }
}

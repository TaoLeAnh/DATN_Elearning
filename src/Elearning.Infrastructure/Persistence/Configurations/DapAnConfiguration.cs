using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class DapAnConfiguration : IEntityTypeConfiguration<DapAn>
    {
        public void Configure(EntityTypeBuilder<DapAn> builder)
        {
            builder.ToTable("DapAn");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NoiDung)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(x => x.HinhAnhUrl)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(x => x.LaDapAnDung)
                .IsRequired();

            builder.HasOne(x => x.CauHoi)
                .WithMany(x => x.DapAns)
                .HasForeignKey(x => x.CauHoiId)
                .OnDelete(DeleteBehavior.Cascade); // Cực kỳ quan trọng: Xóa Câu Hỏi -> Xóa Đáp Án
        }
    }
}

using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class CauHoiKyThiConfiguration : IEntityTypeConfiguration<CauHoiKyThi>
    {
        public void Configure(EntityTypeBuilder<CauHoiKyThi> builder)
        {
            builder.ToTable("CauHoiKyThi");

            builder.HasKey(x => x.Id);

            builder
            .HasOne(x => x.CauHoi)
            .WithMany(x => x.CauHoiKyThis)
            .HasForeignKey(x => x.CauHoiId)
            .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.KyThi)
                .WithMany(x => x.CauHoiKyThis)
                .HasForeignKey(x => x.KyThiId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

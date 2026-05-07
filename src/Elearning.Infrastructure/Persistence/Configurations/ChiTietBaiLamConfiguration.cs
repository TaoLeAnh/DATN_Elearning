using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class ChiTietBaiLamConfiguration : IEntityTypeConfiguration<ChiTietBaiLam>
    {
        public void Configure(EntityTypeBuilder<ChiTietBaiLam> builder)
        {
            builder.ToTable("ChiTietBaiLam");

            builder.HasKey(x => x.Id);

            builder
                .HasOne(x => x.BaiLam)
                .WithMany(x => x.ChiTietBaiLams)
                .HasForeignKey(x => x.BaiLamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.CauHoi)
                .WithMany(x => x.ChiTietBaiLams)
                .HasForeignKey(x => x.CauHoiId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

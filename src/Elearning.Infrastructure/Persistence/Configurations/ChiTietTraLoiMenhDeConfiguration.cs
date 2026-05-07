using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class ChiTietTraLoiMenhDeConfiguration : IEntityTypeConfiguration<ChiTietTraLoiMenhDe>
    {
        public void Configure(EntityTypeBuilder<ChiTietTraLoiMenhDe> builder)
        {
            builder.ToTable("ChiTietTraLoiMenhDe");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.LuaChonCuaHocVien)
                .IsRequired();

            // Móc nối với ChiTietBaiLam
            builder.HasOne(x => x.ChiTietBaiLam)
                .WithMany(x => x.ChiTietTraLoiMenhDes)
                .HasForeignKey(x => x.ChiTietBaiLamId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa bài làm thì xóa luôn chi tiết này

            // Móc nối với MenhDeDungSai
            builder.HasOne(x => x.MenhDeDungSai)
                .WithMany(x => x.ChiTietTraLoiMenhDes)
                .HasForeignKey(x => x.MenhDeDungSaiId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

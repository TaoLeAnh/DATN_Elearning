using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class LogViPhamConfiguration : IEntityTypeConfiguration<LogViPham>
    {
        public void Configure(EntityTypeBuilder<LogViPham> builder)
        {
            builder.ToTable("LogViPham");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.LoaiViPham)
                .IsRequired();

            builder.Property(x => x.ThoiDiemViPham)
                .IsRequired();

            // Cho phép ghi chú dài tối đa 500 ký tự (có thể NULL nếu không cần giải thích thêm)
            builder.Property(x => x.ChiTiet)
                .IsRequired(false)
                .HasMaxLength(500);

            // Móc nối 1-N với bảng BaiLam
            builder.HasOne(x => x.BaiLam)
                .WithMany(x => x.LogViPhams)
                .HasForeignKey(x => x.BaiLamId)
                .OnDelete(DeleteBehavior.Cascade); // CỰC QUAN TRỌNG: Xóa bài làm -> Xóa luôn log rác
        }
    }
}

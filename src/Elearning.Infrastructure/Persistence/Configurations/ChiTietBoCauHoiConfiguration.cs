using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class ChiTietBoCauHoiConfiguration : IEntityTypeConfiguration<ChiTietBoCauHoi>
    {
        public void Configure(EntityTypeBuilder<ChiTietBoCauHoi> builder)
        {
            builder.ToTable("ChiTietBoCauHoi");

            builder.HasKey(x => x.Id);

            // Chống trùng lặp: Một câu hỏi chỉ xuất hiện 1 lần trong 1 Bộ câu hỏi
            builder.HasIndex(x => new { x.BoCauHoiOnTapId, x.CauHoiId })
                .IsUnique()
                .HasDatabaseName("IX_Unique_BoCauHoi_CauHoi");

            // Ràng buộc với Câu hỏi (Restrict)
            builder.HasOne(x => x.CauHoi)
                .WithMany(x => x.ChiTietBoCauHois)
                .HasForeignKey(x => x.CauHoiId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa Câu Hỏi từ Ngân hàng nếu nó đang nằm trong 1 Bộ Đề

            // Ràng buộc với Bộ Đề (Cascade)
            builder.HasOne(x => x.BoCauHoiOnTap)
                .WithMany(x => x.ChiTietBoCauHois)
                .HasForeignKey(x => x.BoCauHoiOnTapId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Bộ Đề -> Xóa các record cấu hình bên trong nó
        }
    }
}

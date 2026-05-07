using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class BaiLamConfiguration : IEntityTypeConfiguration<BaiLam>
    {
        public void Configure(EntityTypeBuilder<BaiLam> builder)
        {
            builder.ToTable("BaiLam");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ThoiDiemBatDau)
                .IsRequired();

            // Cho phép NULL vì khi mới làm bài chưa có giờ nộp
            builder.Property(x => x.ThoiDiemNop)
                .IsRequired(false);

            builder.Property(x => x.Diem)
                .IsRequired();

            builder.Property(x => x.SoCauDung)
                .IsRequired();

            builder.Property(x => x.TrangThai)
                .IsRequired();

            // 1. ĐÃ SỬA: Thêm IsRequired(false) để EF Core biết KyThiId là Nullable
            builder.HasOne(x => x.KyThi)
                .WithMany(x => x.BaiLams)
                .HasForeignKey(x => x.KyThiId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. THÊM MỚI: Cấu hình Khóa ngoại cho BoCauHoiOnTap
            builder.HasOne(x => x.BoCauHoiOnTap)
                .WithMany() // Để trống trong ngoặc nếu bạn không khai báo ICollection<BaiLam> bên Entity BoCauHoiOnTap
                .HasForeignKey(x => x.BoCauHoiOnTapId)
                .IsRequired(false) // Cho phép Null vì nếu là bài Thi thật thì trường này rỗng
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.NguoiDung)
                .WithMany(x => x.BaiLams)
                .HasForeignKey(x => x.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

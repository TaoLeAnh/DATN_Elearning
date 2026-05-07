using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class CauHoiConfiguration : IEntityTypeConfiguration<CauHoi>
    {
        public void Configure(EntityTypeBuilder<CauHoi> builder)
        {
            builder.ToTable("CauHoi");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NoiDung)
                .IsRequired(false)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.HinhAnhUrl)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(x => x.GiaiThich)
                .IsRequired(false)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ChuDe)
                .IsRequired()
                .HasMaxLength(200);

            // --- BỔ SUNG CẤU HÌNH CHO TRƯỜNG MÔN HỌC ---
            builder.Property(x => x.MonHoc)
                .IsRequired(false); // Cho phép null vì có những câu hỏi chỉ thuộc khóa học nội bộ
            // -------------------------------------------

            // --- THÊM CẤU HÌNH CHO KHÓA NGOẠI KHOA HOC ---
            builder.Property(x => x.KhoaHocId)
                .IsRequired(false);

            builder.HasOne(x => x.KhoaHoc)
                .WithMany() // Để trống bên trong nếu class KhoaHoc chưa có ICollection<CauHoi>
                .HasForeignKey(x => x.KhoaHocId)
                .OnDelete(DeleteBehavior.Restrict); // CỰC KỲ QUAN TRỌNG: Chống xóa nhầm
            // ----------------------------------------------

            builder.HasOne(x => x.GiangVien)
                .WithMany(x => x.CauHois)
                .HasForeignKey(x => x.GiangVienId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

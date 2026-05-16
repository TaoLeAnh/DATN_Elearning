using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class MenhDeDungSaiConfiguration : IEntityTypeConfiguration<MenhDeDungSai>
    {
        public void Configure(EntityTypeBuilder<MenhDeDungSai> builder)
        {
            builder.ToTable("MenhDeDungSai");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NoiDung)
                .IsRequired(false)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.HinhAnhUrl)
                .HasMaxLength(1000) // URL thường dài nên cho hẳn 1000 cho thoải mái
                .IsRequired(false);

            builder.HasOne(x => x.CauHoi)
                .WithMany(x => x.MenhDeDungSais) // <--- SỬA CHỖ NÀY: Điền thêm x => x.MenhDeDungSais
                .HasForeignKey(x => x.CauHoiId)
                .OnDelete(DeleteBehavior.Cascade); // Mệnh đề là con của Câu hỏi, Câu hỏi xóa thì mệnh đề cũng nên bay màu
        }
    }
}

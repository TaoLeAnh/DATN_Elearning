using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Persistence.Configurations
{
    public class DapAnDienKetQuaConfiguration : IEntityTypeConfiguration<DapAnDienKetQua>
    {
        public void Configure(EntityTypeBuilder<DapAnDienKetQua> builder)
        {
            builder.ToTable("DapAnDienKetQua");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.CauHoi)
                .WithMany()
                .HasForeignKey(x => x.CauHoiId);
        }
    }
}

using Elearning.Shared.Commons.Model.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elearning.Infrastructure.Extensions
{
    public static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// Cấu hình chung cho các thuộc tính của BaseDomainEntity
        /// </summary>
        public static void ConfigureBaseDomainEntity<T>(this EntityTypeBuilder<T> builder)
            where T : BaseDomainEntity
        {

            // Cấu hình khóa chính (Id)
            builder.HasKey(e => e.Id);

            // //Oracle
            //builder.Property(e => e.Id)
            //    .IsRequired()
            //    .HasColumnName("ID")
            //    .HasConversion(
            //        g => g.ToString("D"),      // Guid -> "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
            //        s => Guid.Parse(s))        // string -> Guid
            //    .HasColumnType("VARCHAR2(36 CHAR)")
            //    .HasMaxLength(36)
            //    .IsUnicode(false)
            //    .ValueGeneratedNever();

            //SQL MS
            builder.Property(e => e.Id)
                .HasColumnName("ID")
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWSEQUENTIALID()");


            // Cấu hình Created
            builder.Property(e => e.Created)
                .HasColumnName("CREATED")
                .IsRequired();

            builder.Property(e => e.LastModified)
              .HasColumnName("LASTMODIFIED")
              .IsRequired();

            //// DepartmentId ORACLE
            //builder.Property(e => e.DepartmentId)
            //    .IsRequired()
            //    .HasColumnName("DEPARTMENTID")
            //    .HasConversion(
            //        g => g.ToString("D"),   // Guid -> string
            //        s => Guid.Parse(s))     // string -> Guid
            //    .HasColumnType("VARCHAR2(36 CHAR)")
            //    .HasMaxLength(36)
            //    .IsUnicode(false);

            //// CreatedBy
            //builder.Property(e => e.CreatedBy)
            //    .IsRequired()
            //    .HasColumnName("CREATEDBY")
            //    .HasConversion(
            //        g => g.ToString("D"),
            //        s => Guid.Parse(s))
            //    .HasColumnType("VARCHAR2(36 CHAR)")
            //    .HasMaxLength(36)
            //    .IsUnicode(false);

            //builder.Property(e => e.LastModifiedBy)
            //     .IsRequired()
            //     .HasColumnName("LASTMODIFIEDBY")
            //     .HasConversion(
            //         g => g.ToString("D"),
            //         s => Guid.Parse(s))
            //     .HasColumnType("VARCHAR2(36 CHAR)")
            //     .HasMaxLength(36)
            //     .IsUnicode(false);
            //builder.Property(e => e.LastModifiedBy)
            //    .HasColumnName("LASTMODIFIEDBY")
            //    .IsRequired()
            //    .IsRequired()
            //    .HasMaxLength(256);


            builder.Property(e => e.DepartmentId).HasColumnName("DEPARTMENTID").IsRequired();
            builder.Property(e => e.CreatedBy).HasColumnName("CREATEDBY").IsRequired().HasMaxLength(256);
            builder.Property(e => e.LastModifiedBy).HasColumnName("LASTMODIFIEDBY").IsRequired().HasMaxLength(256);

            // Cấu hình ModerationStatus
            builder.Property(e => e.ModerationStatus)
                .HasColumnName("MODERATIONSTATUS")
                .HasConversion<int>()
                .IsRequired();
            builder.HasIndex(e => e.Created);
            builder.HasIndex(e => e.ModerationStatus);

        }

        /// <summary>
        /// Cấu hình chung cho các thuộc tính của IntermediaryEntity
        /// </summary>
        public static void ConfigureIntermediaryEntity<T>(this EntityTypeBuilder<T> builder)
            where T : IntermediaryEntity
        {
            // Cấu hình khóa chính (Id)
            builder.HasKey(e => e.Id);
            //builder.Property(e => e.Id)
            //   .IsRequired()
            //   .HasColumnName("ID")
            //   .HasConversion(
            //       g => g.ToString("D"),
            //       s => Guid.Parse(s))
            //   .HasColumnType("VARCHAR2(36 CHAR)")
            //   .HasMaxLength(36)
            //   .IsUnicode(false)
            //   .ValueGeneratedNever();

            //SQL MS
            builder.Property(e => e.Id)
                .HasColumnName("ID")
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWSEQUENTIALID()");


            // Cấu hình Created
            builder.Property(e => e.Created)
                .HasColumnName("CREATED")
                .IsRequired();

            builder.HasIndex(e => e.Created);

        }
    }
}

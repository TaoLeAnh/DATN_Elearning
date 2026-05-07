using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Elearning.Publising.Infrastructure.Persistence.Context
{
    public class AppDbContext : AuditableDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<KhoaHoc> KhoaHocs { get; set; }
        public DbSet<HoSoGiaoVien> HoSoGiaoViens { get; set; }
        public DbSet<DangKyKhoaHoc> DangKyKhoaHocs { get; set; }
        public DbSet<ChuongHoc> ChuongHocs { get; set; }
        public DbSet<MaTranDeThiMacDinh> MaTranDeThiMacDinhs { get; set; }
        public DbSet<BaiHoc> BaiHocs { get; set; }
        public DbSet<TienDoHoc> TienDoHocs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<KhoaHoc>().ToTable("KhoaHoc");
            builder.Entity<HoSoGiaoVien>().ToTable("HoSoGiaoVien");
            builder.Entity<DangKyKhoaHoc>().ToTable("DangKyKhoaHoc");
            builder.Entity<ChuongHoc>().ToTable("ChuongHoc");
            builder.Entity<BaiHoc>().ToTable("BaiHoc");
            builder.Entity<TienDoHoc>().ToTable("TienDoHoc");
            builder.Entity<MaTranDeThiMacDinh>().ToTable("MaTranDeThiMacDinh");
            // FIX: DB lưu giờ Local sẵn rồi, chỉ cần SpecifyKind = Unspecified
            // Không ToLocalTime() vì sẽ bị cộng thêm +7
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v,
                            v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified) // ← SỬA
                        ));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                            v => v,
                            v => v.HasValue
                                ? DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified) // ← SỬA
                                : null
                        ));
                    }
                }
            }

            base.OnModelCreating(builder);
        }
    }
}

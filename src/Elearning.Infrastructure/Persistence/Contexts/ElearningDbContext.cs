using Elearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Elearning.Infrastructure.Persistence.Contexts
{
    public class ElearningDbContext : AuditableDbContext
    {
        public ElearningDbContext(DbContextOptions<ElearningDbContext> options)
            : base(options)
        {
        }

        public DbSet<NguoiDung> NguoiDungs { get; set; }

        public DbSet<KhoaHoc> KhoaHocs { get; set; }

        public DbSet<ChuongHoc> ChuongHocs { get; set; }

        public DbSet<BaiHoc> BaiHocs { get; set; }

        public DbSet<DangKyKhoaHoc> DangKyKhoaHocs { get; set; }

        public DbSet<TienDoHoc> TienDoHocs { get; set; }

        public DbSet<CauHoi> CauHois { get; set; }

        public DbSet<DapAn> DapAns { get; set; }

        public DbSet<KyThi> KyThis { get; set; }

        public DbSet<BaiLam> BaiLams { get; set; }

        public DbSet<ChiTietBaiLam> ChiTietBaiLams { get; set; }

        public DbSet<BoCauHoiOnTap> BoCauHoiOnTaps { get; set; }

        public DbSet<ChiTietBoCauHoi> ChiTietBoCauHois { get; set; }

        public DbSet<MenhDeDungSai> MenhDeDungSais { get; set; }

        public DbSet<DapAnDienKetQua> DapAnDienKetQuas { get; set; }

        public DbSet<CauHoiKyThi> CauHoiKyThis { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElearningDbContext).Assembly);

            foreach (var relationship in modelBuilder.Model
                     .GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
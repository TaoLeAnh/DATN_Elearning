using Elearning.Shared.Commons.Extensions;
using Elearning.Shared.Commons.Model.SQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Infrastructure.Persistence.Context
{
    public abstract class AuditableDbContext : DbContext
    {
        protected AuditableDbContext(DbContextOptions options) : base(options)
        {
        }

        private void ValidateIds(Guid userId, Guid departmentId)
        {
            //if (userId == Guid.Empty)
            //    throw new ArgumentException("UserId cannot be empty", nameof(userId));

            //if (departmentId == Guid.Empty)

            //    throw new ArgumentException("DepartmentId cannot be empty", nameof(departmentId));
        }

        private void ApplyAuditData(Guid userId, Guid departmentId)
        {

            ///tam fix vao day
            if (departmentId == Guid.Empty)
                departmentId = new Guid("12345678-1234-5678-1234-567812345678");
            if (userId == Guid.Empty)
                userId = new Guid("12345678-1234-5678-1234-567812345678");

            foreach (var entry in ChangeTracker.Entries<BaseDomainEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.FillDataForInsert(userId, departmentId);
                        break;
                    case EntityState.Modified:
                        entry.Entity.FillDataForUpdate(userId);
                        break;
                }
            }
            foreach (var entry in ChangeTracker.Entries<IntermediaryEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.FillDataForInsert(userId);
                        break;
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess = true)
        {
            throw new NotSupportedException("Use SaveChanges(Guid userId, Guid departmentId) instead");
        }



        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Use SaveChangesAsync(Guid userId, Guid departmentId) instead");
        }

        public virtual int SaveChanges(Guid userId, Guid departmentId)
        {
            try
            {
                ValidateIds(userId, departmentId);
                ApplyAuditData(userId, departmentId);
                NormalizeDateTimes();
                return base.SaveChanges(true);
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException($"SQL Error: {sqlEx.Message}", sqlEx.Number);
            }
            catch (DbUpdateException dbEx)
            {
                var sqlEx = dbEx.InnerException as SqlException;
                throw new DatabaseException($"DbUpdate Error: {dbEx.Message}", sqlEx?.Number);
            }

        }

        public virtual async Task<int> SaveChangesAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateIds(userId, departmentId);
                ApplyAuditData(userId, departmentId);
                NormalizeDateTimes();
                return await base.SaveChangesAsync(true, cancellationToken);
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException($"SQL Error: {sqlEx.Message}", sqlEx.Number);
            }
            catch (DbUpdateException dbEx)
            {
                var sqlEx = dbEx.InnerException as SqlException;
                throw new DatabaseException($"DbUpdate Error: {dbEx.Message}", sqlEx?.Number);
            }

        }

        private void NormalizeDateTimes()
        {
            foreach (var entry in ChangeTracker.Entries()
                         .Where(e => e.State is EntityState.Added or EntityState.Modified))
            {
                foreach (var prop in entry.Properties
                             .Where(p => p.Metadata.ClrType == typeof(DateTime) ||
                                         p.Metadata.ClrType == typeof(DateTime?)))
                {
                    if (prop.CurrentValue is DateTime dt && dt.Kind != DateTimeKind.Utc)
                        prop.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                }
            }
        }
    }
}

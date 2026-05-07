using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Shared.Commons.Extensions;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Interfaces.SQL;
using Microsoft.EntityFrameworkCore;

namespace Elearning.Infrastructure.Repository.Base
{
    public class BaseUnitOfWork : IBaseUnitOfWork
    {
        private ITransaction? _currentTransaction;
        private readonly ElearningDbContext _context;
        private readonly IRequestContext _requestContext;
        private bool _disposed;

        public BaseUnitOfWork(
            ElearningDbContext context,
            IRequestContext requestContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentTransaction = null;
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
            #region Initialize repositories


            #endregion
        }
        public int Complete(Guid UserId = default, Guid DepartmentId = default)
        {
            ValidateAndUpdateIds(ref UserId, ref DepartmentId);
            return _context.SaveChanges(UserId, DepartmentId);
        }

        public async Task<int> CompleteAsync(Guid UserId = default, Guid DepartmentId = default)
        {
            ValidateAndUpdateIds(ref UserId, ref DepartmentId);
            return await _context.SaveChangesAsync(UserId, DepartmentId);
        }

        public IEnumerable<T> ExecuteSqlRaw<T>(string sql) where T : class
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);
            return _context.Database.SqlQueryRaw<T>(sql).AsEnumerable();
        }

        public async Task<ITransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return _currentTransaction;

            var transaction = await _context.Database.BeginTransactionAsync();
            _currentTransaction = new EfCoreTransaction(transaction);
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No active transaction found");
            try
            {
                Guid UserId = Guid.Empty, DepartmentId = Guid.Empty;
                ValidateAndUpdateIds(ref UserId, ref DepartmentId);
                await _context.SaveChangesAsync(UserId, DepartmentId);
                await _currentTransaction.CommitAsync();
            }
            finally
            {
                DisposeCurrentTransaction();
            }
        }


        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
                return;

            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                DisposeCurrentTransaction();
            }
        }

        private void DisposeCurrentTransaction()
        {
            if (_currentTransaction == null) return;

            _currentTransaction.Dispose();
            _currentTransaction = null;
        }

        private void ValidateAndUpdateIds(ref Guid UserId, ref Guid DepartmentId)
        {
            if (!_requestContext.CurrentUser.UserId.IsEmpty())
            {
                UserId = _requestContext.CurrentUser.UserId;
            }
            if (!_requestContext.CurrentUser.DepartmentId.IsEmpty())
            {
                DepartmentId = _requestContext.CurrentUser.DepartmentId;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                DisposeCurrentTransaction();
                _context.Dispose();
            }

            _disposed = true;
        }

        public async Task<int> ExecuteNonQueryAsync(string sql)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task<int> ExecuteNonQueryInterpolatedAsync(FormattableString sql)
        {
            return await _context.Database.ExecuteSqlInterpolatedAsync(sql);
        }
    }
}

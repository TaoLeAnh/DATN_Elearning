using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Interfaces.SQL
{
    public interface IBaseUnitOfWork : IDisposable
    {

        /// <summary>
        /// Tạo phiên làm việc với transestion
        /// using (var transaction = await _unitOfWork.BeginTransactionAsync())
        /// {
        /// try
        /// {
        ///     Thực hiện các thao tác
        /// await _unitOfWork.CompleteAsync();
        /// await _unitOfWork.CommitTransactionAsync();
        /// }
        /// catch
        ///{
        ///
        ///    throw;
        ///}
        ///}
        /// </summary>
        /// <returns></returns>
        Task<ITransaction> BeginTransactionAsync();

        /// <summary>
        /// await _unitOfWork.CommitTransactionAsync();
        /// </summary>
        /// <returns></returns>
        Task CommitTransactionAsync();

        /// <summary>
        /// await _unitOfWork.RollbackTransactionAsync();
        /// </summary>
        /// <returns></returns>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Thực thi câu lệnh SQL dạng raw (chuỗi thường).
        /// </summary>
        Task<int> ExecuteNonQueryAsync(string sql);

        /// <summary>
        /// Thực thi câu lệnh SQL dạng interpolated (an toàn hơn tránh SQL injection).
        /// </summary>
        Task<int> ExecuteNonQueryInterpolatedAsync(FormattableString sql);


        IEnumerable<T> ExecuteSqlRaw<T>(string sqls) where T : class;
        Task<int> CompleteAsync(Guid UserId = default, Guid DepartmentId = default);
        int Complete(Guid UserId = default, Guid DepartmentId = default);


    }
}

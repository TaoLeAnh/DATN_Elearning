using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Elearning.Infrastructure.Extensions
{
    public sealed class SlowQueryInterceptor : DbCommandInterceptor
    {
        private readonly TimeSpan _threshold;
        private readonly ILogger<SlowQueryInterceptor> _log;

        public SlowQueryInterceptor(TimeSpan threshold, ILogger<SlowQueryInterceptor> log)
            => (_threshold, _log) = (threshold, log);


        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData data, DbDataReader result)
        { LogIfSlow(command, data); return result; }

        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData data, int result)
        { LogIfSlow(command, data); return result; }

        public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData data, object? result)
        { LogIfSlow(command, data); return result; }

        private void LogIfSlow(DbCommand cmd, CommandExecutedEventData data)
        {
            if (data.Duration <= _threshold) return;

            try
            {
                var parameters = cmd.Parameters.Cast<DbParameter>()
                    .Select(p => new { p.ParameterName, Value = p.Value?.ToString() ?? "NULL", p.DbType });

                _log.LogWarning("SLOW SQL {Duration}ms | Context: {Context} | Tag: {Tag}\n{SQL}\nParams: {@Parameters}",
                    data.Duration.TotalMilliseconds,
                    data.Context?.GetType().Name,
                    GetQueryTag(cmd),
                    cmd.CommandText,
                    parameters);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error logging slow query");
            }
        }

        private static string GetQueryTag(DbCommand cmd)
        {
            var sql = cmd.CommandText ?? string.Empty;

            // Ưu tiên block comment đầu câu lệnh: /* MyTag */
            var i = sql.IndexOf("/*");
            if (i >= 0)
            {
                var j = sql.IndexOf("*/", i + 2);
                if (j > i) return sql.Substring(i + 2, j - i - 2).Trim();
            }

            // Fallback: dòng bắt đầu bằng --
            foreach (var line in sql.Split('\n'))
            {
                var t = line.TrimStart();
                if (t.StartsWith("--")) return t.TrimStart('-', ' ').Trim();
            }
            return "Unknown";
        }
    }
}

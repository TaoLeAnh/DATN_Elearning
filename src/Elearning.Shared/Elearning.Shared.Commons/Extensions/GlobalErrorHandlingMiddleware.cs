using Elearning.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Commons.Model.SQL;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// Khởi tạo
        /// </summary>
        /// <param name="next"></param>
        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        /// <summary>
        /// InvokeAsync
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidInputException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;

                var errorResponse = new SiResponse<DefaultClass>
                {
                    Success = false,
                    Message = "Thao tác thất bại",
                    BuildQuery = null,
                    Data = null,
                    Errors = new Error
                    {
                        Id = new[]
                        {
                            "Dữ liệu đầu vào không hợp lệ!",
                            ex.Message
                        }
                    },
                    StatusCode = StatusCode.BadRequest
                };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
            catch (DatabaseException ex)
            {
                // Chỉ log những error code cần thiết cho monitoring
                var shouldLog = ex.ErrorCode switch
                {
                    2601 or 2627 => false, // Duplicate key - business logic, không cần log
                    547 => false,          // Foreign key constraint - expected behavior
                    515 => false,          // Not null constraint - validation issue
                    -2 => true,           // Timeout - cần monitor performance
                    2 => true,            // Connection failed - infrastructure issue
                    18456 => true,        // Login failed - security concern
                    _ when ex.TechnicalMessage.ToLower().Contains("deadlock") => true, // Performance issue
                    _ => true             // Unknown errors cần investigate
                };

                if (shouldLog)
                {
                    Log.Error(ex, "Database error requiring attention - Code: {ErrorCode}", ex.ErrorCode);
                }
                else
                {
                    Log.Debug(ex, "Database constraint violation - Code: {ErrorCode}", ex.ErrorCode);
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var errorResponse = new SiResponse<DefaultClass>
                {
                    Success = false,
                    Message = "Thao tác thất bại",
                    BuildQuery = null,
                    Data = null,
                    Errors = new Error
                    {
                        Id = new[] { ex.UserMessage }
                    },
                    StatusCode = StatusCode.InternalServerError
                };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;

                var errorResponse = new SiResponse<DefaultClass>
                {
                    Success = false,
                    Message = "Thao tác thất bại",
                    BuildQuery = null,
                    Data = null,
                    Errors = new Error
                    {
                        Id = new[]
                        {
                            "Thao tác thất bại!",
                            ex.Message
                        }
                    },
                    StatusCode = StatusCode.BadRequest
                };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
        }


    }
}

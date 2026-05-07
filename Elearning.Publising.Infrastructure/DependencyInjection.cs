using Elearning.Domain.Interfaces;
using Elearning.Infrastructure.Extensions;
using Elearning.Publising.Infrastructure.Persistence.Context;
using Elearning.Publising.Infrastructure.Repositories;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrustructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("SQLConnection")
                ?? throw new InvalidOperationException("Connection string 'SQLConnection' not found.");

            services.AddDbContext<AppDbContext>((sp, opt) =>
            {
                var interceptor = new SlowQueryInterceptor(
                    TimeSpan.FromMilliseconds(300),
                    sp.GetRequiredService<ILogger<SlowQueryInterceptor>>());
                var logger = sp.GetRequiredService<ILogger<AppDbContext>>();
                opt.UseSqlServer(connectionString)
                .AddInterceptors(interceptor)
                .EnableSensitiveDataLogging(false);
#if DEBUG
                opt.LogTo(msg => logger.LogInformation("{EFCore}", msg),
                          (eventId, level) => eventId.Id == RelationalEventId.CommandExecuted.Id,
                          DbContextLoggerOptions.SingleLine | DbContextLoggerOptions.LocalTime);
#endif
            });

            services.AddScoped<IDbContextTransaction>(provider => null!);
            services.AddScoped<IRequestContext, RequestContext>();

            services.AddHttpClient<ICallServiceRegistry, CallServiceRegistryAPI>();

            #region Đăng ký EF
            services.AddScoped<IUnitOfWorkPublising, UnitOfWorkPublising>();
            #endregion


            return services;
        }
    } 
}

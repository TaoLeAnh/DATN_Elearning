using Elearning.Domain.Interfaces;
using Elearning.Infrastructure.Extensions;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using Elearning.Infrastructure.Repository.UnitOfWorks;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Commons.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Elearning.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrustructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            string connectionString = configuration.GetConnectionString("SQLConnection")
                ?? throw new InvalidOperationException("Connection string 'SQLConnection' not found.");
            services.AddDbContext<ElearningDbContext>((sp, opt) =>
            {
                var interceptor = new SlowQueryInterceptor(
                    TimeSpan.FromMilliseconds(300),
                    sp.GetRequiredService<ILogger<SlowQueryInterceptor>>());

                var logger = sp.GetRequiredService<ILogger<ElearningDbContext>>();

                opt.UseSqlServer(connectionString)
                   .AddInterceptors(interceptor)
                   .EnableSensitiveDataLogging(false);

#if DEBUG
                opt.LogTo(
                       msg => logger.LogInformation("{EFCore}", msg),
                       (eventId, level) => eventId.Id == RelationalEventId.CommandExecuted.Id,
                       DbContextLoggerOptions.SingleLine | DbContextLoggerOptions.LocalTime);
#endif



            });

            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseNpgsql(connectionString, npgsqlOptions =>
            //    {

            //        npgsqlOptions.SetPostgresVersion(new Version(16, 0));
            //        npgsqlOptions.EnableRetryOnFailure(
            //            maxRetryCount: 5,
            //            maxRetryDelay: TimeSpan.FromSeconds(10),
            //            errorCodesToAdd: null);
            //    })
            //// .UseSnakeCaseNamingConvention()  <-- remove this
            //);
            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseOracle(connectionString,
            //        oracleOptions =>
            //        {
            //            oracleOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19);
            //        });
            //    options.EnableSensitiveDataLogging();
            //    options.LogTo(Console.WriteLine, LogLevel.Information);
            //});




            services.AddScoped<IDbContextTransaction>(provider => null!);
            services.AddScoped<ITransaction, EfCoreTransaction>();
            services.AddScoped<IRequestContext, RequestContext>();





            services.AddSingleton<ICacheService>(provider =>
            {
                string connectionString = configuration["Redis:Connection"]
                    ?? throw new InvalidOperationException("Connection string 'Redis:Connection' not found.");

                string prefixKey = configuration["Redis:CachePrefixKey"]
                    ?? throw new InvalidOperationException("'CachePrefixKey' not found.");

                return new CacheService(connectionString, prefixKey);
            });



            //            string ElasticConnection = configuration["Elastic:HostName"]
            //                ?? throw new InvalidOperationException("Connection string 'Elastic:HostName' not found.");
            //            string ElasticUserName = configuration["Elastic:UserName"] ?? string.Empty;
            //            string ElasticPassword = configuration["Elastic:Password"] ?? string.Empty;


            //            var settings = new ElasticsearchClientSettings(new Uri(ElasticConnection));
            //            if (!string.IsNullOrEmpty(ElasticPassword) && !string.IsNullOrEmpty(ElasticUserName))
            //                settings.Authentication(new BasicAuthentication(ElasticUserName, ElasticPassword));
            //#if DEBUG
            //            settings.EnableDebugMode();
            //#endif
            //            var client = new ElasticsearchClient(settings);
            //            services.AddSingleton(client);

            //            #region cấu hình s3
            //            string S3Endpoint = configuration["S3:S3ConnectionString"] ?? throw new InvalidOperationException("'S3ConnectionString' not found.");
            //            string S3AccessKey = configuration["S3:S3AccessKeyString"] ?? throw new InvalidOperationException("'S3AccessKeyString' not found.");
            //            string S3Secretkey = configuration["S3:S3SecretkeyString"] ?? throw new InvalidOperationException("'S3SecretkeyString' not found.");
            //            bool ForcePathStyle = bool.TryParse(configuration["S3:ForcePathStyle"], out var fps) ? fps : true;
            //            bool UseHttp = bool.TryParse(configuration["S3:UseHttp"], out var uh) ? uh : true;
            //            string AuthenticationRegion = configuration["S3:AuthenticationRegion"] ?? "hn";


            //services
            //    .AddMinio(configureClient => configureClient
            //    .WithEndpoint(S3Endpoint)
            //    .WithCredentials(S3AccessKey, S3Secretkey)
            //    .WithSSL(false)
            //    .Build());
            //services.AddScoped<IS3Repository, S3Repository>();

            //Bộ ngoại giao dùng Hyperstone nên dùng thư viện này
            //Amazon.AWSConfigsS3.DisableDefaultChecksumValidation = true;
            //var config = new AmazonS3Config
            //{
            //    ServiceURL = S3Endpoint,
            //    ForcePathStyle = ForcePathStyle,
            //    UseHttp = UseHttp
            //};
            //if (!string.IsNullOrEmpty(AuthenticationRegion))
            //    config.AuthenticationRegion = AuthenticationRegion;

            //services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(S3AccessKey, S3Secretkey, config));
            //services.AddScoped<IS3Repository, S3AmazonRepository>();


            //#endregion


            services.AddHttpClient<ICallServiceRegistry, CallServiceRegistryAPI>();
            //services.AddEmailSender(configuration);

            #region Đăng ký EF

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            #endregion



            #region Đăng ký các job ngầm của phần mềm


            //services.AddHostedService<SystemConfigBackgroundService>();//init nghiệp vụ
            //services.AddHostedService<SeedingDataBackgroundService>();// init giao dữ liệu

            //services.AddHostedService<CleanAndReindexHotNewsBgService>();

            //services.AddSingleton<LoggingThaoTacQueue>(sp => new LoggingThaoTacQueue(1000));
            //services.AddHostedService<LoggingBackgroundService>();
            //services.AddHostedService<AnalyticsLogTimKiemBgService>();
            //services.AddHostedService<CleanSearchLogBgService>();

            #endregion

            return services;
        }
    }
}
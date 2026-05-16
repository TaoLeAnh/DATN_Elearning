using Elearning.Publising.Application.Interfaces;
using Elearning.Publising.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServicesDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            AddService(services);
            return services;
        }
        private static void AddService(IServiceCollection services)
        {
            services.AddScoped<IKhoaHocService, KhoaHocService>();
            services.AddScoped<IBoCauHoiOnTapService, BoCauHoiOnTapService>();
            services.AddScoped<IAuthPublisingService, AuthPublisingService>();
            services.AddScoped<IKyThiService, KyThiService>();
            services.AddScoped<ITienDoHocService, TienDoHocService>();
            services.AddScoped<IBaiLamService, BaiLamService>();
            services.AddScoped<IHoSoGiaoVienService, HoSoGiaoVienService>();
            services.AddScoped<IDangKyKhoaHocService, DangKyKhoaHocService>();
            services.AddScoped<IChatbotService, ChatBotService>();
            services.AddSingleton<IExamQueueService, ExamQueueService>();
            services.AddHostedService<ExamScoringBackgroundWorker>();

        }
    }
}

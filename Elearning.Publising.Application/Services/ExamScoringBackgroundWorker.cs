using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Services
{
    public sealed class ExamScoringBackgroundWorker : BackgroundService
    {
        private const int MaxRetry = 3; // Thử chấm lại tối đa 3 lần nếu có lỗi DB

        private readonly IExamQueueService _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExamScoringBackgroundWorker> _logger;

        public ExamScoringBackgroundWorker(
            IExamQueueService queue,
            IServiceScopeFactory scopeFactory,
            ILogger<ExamScoringBackgroundWorker> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExamScoringBackgroundWorker started.");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ExamQueueItem item;
                    try
                    {
                        item = await _queue.DequeueBaiNopAsync(stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break; // Dừng an toàn khi tắt Web Server
                    }

                    // Tách hàm xử lý riêng để nếu lỗi 1 bài không làm chết vòng lặp
                    await ProcessItemAsync(item, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "ExamScoringBackgroundWorker crashed unexpectedly.");
                throw;
            }

            _logger.LogInformation("ExamScoringBackgroundWorker stopped.");
        }

        private async Task ProcessItemAsync(ExamQueueItem item, CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var kyThiService = scope.ServiceProvider.GetRequiredService<IKyThiService>();

            try
            {
                _logger.LogInformation("Đang chấm điểm bài làm (Lần thử: {RetryCount})", item.RetryCount + 1);

                await kyThiService.NopBaiThiAsync(item.Request);

                _logger.LogInformation("Chấm điểm THÀNH CÔNG!");
            }
            catch (Exception ex)
            {
                item.RetryCount++;

                if (item.RetryCount < MaxRetry)
                {
                    _logger.LogWarning(ex, "Chấm lỗi. Đang đưa vào Queue thử lại (Lần {RetryCount}/{MaxRetry}).", item.RetryCount, MaxRetry);
                    await _queue.EnqueueBaiNopAsync(item, stoppingToken);
                }
                else
                {
                    _logger.LogError(ex, "Chấm thất bại sau {MaxRetry} lần thử. Hủy bỏ!", MaxRetry);
                }
            }
        }
    }
}

using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Contracts.Portal.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Elearning.Publising.Application.Services
{
    public class ExamQueueService : IExamQueueService
    {
        private readonly Channel<ExamQueueItem> _queue;

        public ExamQueueService()
        {
            var options = new BoundedChannelOptions(5000)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            _queue = Channel.CreateBounded<ExamQueueItem>(options);
        }

        public async ValueTask EnqueueBaiNopAsync(ExamQueueItem item, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            await _queue.Writer.WriteAsync(item, ct);
        }

        public async ValueTask<ExamQueueItem> DequeueBaiNopAsync(CancellationToken ct = default)
        {
            return await _queue.Reader.ReadAsync(ct);
        }
    }
}

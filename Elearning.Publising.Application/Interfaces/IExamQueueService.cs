using Elearning.Shared.Contracts.Portal.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Interfaces
{
    public interface IExamQueueService
    {
        ValueTask EnqueueBaiNopAsync(ExamQueueItem item, CancellationToken ct = default);
        ValueTask<ExamQueueItem> DequeueBaiNopAsync(CancellationToken ct = default);
    }
}

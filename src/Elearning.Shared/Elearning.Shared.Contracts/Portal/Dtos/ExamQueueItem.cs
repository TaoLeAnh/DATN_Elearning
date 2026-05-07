using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class ExamQueueItem
    {
        public NopBaiRequest Request { get; set; } = new NopBaiRequest();

        // Dùng để đếm số lần Background Worker đã cố gắng chấm điểm bài này
        public int RetryCount { get; set; } = 0;
    }
}

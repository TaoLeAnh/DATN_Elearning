using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Elearning.Application.Services
{
    public class TranscriptWorkItem
    {
        public Guid BaiHocId { get; set; }
        public string VideoUrl { get; set; }
    }

    public class VideoTranscriptQueue
    {
        // Channel giống như một cái ống, nhét ở đầu này, hút ở đầu kia
        private readonly Channel<TranscriptWorkItem> _queue;

        public VideoTranscriptQueue()
        {
            // Cấu hình ống chứa tối đa 1000 công việc cùng lúc
            var options = new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<TranscriptWorkItem>(options);
        }

        // Hàm để BaiHocService nhét link vào hàng đợi
        public async ValueTask QueueWorkItemAsync(TranscriptWorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            await _queue.Writer.WriteAsync(workItem);
        }

        // Hàm để BackgroundWorker lấy link ra xử lý
        public async ValueTask<TranscriptWorkItem> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}

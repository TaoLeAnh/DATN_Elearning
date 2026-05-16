using Elearning.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YoutubeExplode;

namespace Elearning.Application.Services
{
    public class TranscriptBackgroundWorker : BackgroundService
    {
        private readonly VideoTranscriptQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TranscriptBackgroundWorker> _logger;

        private static readonly Guid SystemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        public TranscriptBackgroundWorker(VideoTranscriptQueue queue, IServiceProvider serviceProvider, ILogger<TranscriptBackgroundWorker> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Công nhân cào phụ đề (YoutubeExplode) đã sẵn sàng...");

            // Khởi tạo YoutubeClient một lần để dùng chung
            var youtube = new YoutubeClient();

            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _queue.DequeueAsync(stoppingToken);
                try
                {
                    string videoId = ExtractVideoId(workItem.VideoUrl);
                    if (string.IsNullOrEmpty(videoId)) continue;

                    _logger.LogInformation($"🔍 Đang xử lý VideoId: {videoId}");

                    // Lấy danh sách các track phụ đề
                    var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(videoId, stoppingToken);

                    // Tìm Tiếng Việt, nếu không có lấy Tiếng Anh, không có nữa thì lấy cái đầu tiên
                    var trackInfo = trackManifest.TryGetByLanguage("vi")
                                 ?? trackManifest.TryGetByLanguage("en")
                                 ?? trackManifest.Tracks.FirstOrDefault();

                    if (trackInfo != null)
                    {
                        // Tải toàn bộ nội dung phụ đề
                        var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo, stoppingToken);

                        string fullText = string.Join(" ", track.Captions
                            .Select(c => c.Text)
                            .Where(text => !string.IsNullOrWhiteSpace(text)));

                        if (!string.IsNullOrWhiteSpace(fullText))
                        {
                            await SaveTaiLieuAI(workItem.BaiHocId, fullText);
                            _logger.LogInformation($"✅ Thành công: {videoId}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"⚠️ Video {videoId} không có phụ đề.");
                        await UpdateStatusMessage(workItem.BaiHocId, "Video không có phụ đề.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Lỗi cào phụ đề video {workItem.VideoUrl}: {ex.Message}");
                    await UpdateStatusMessage(workItem.BaiHocId, "Không thể lấy phụ đề lúc này do bị chặn hoặc video lỗi.");
                }

                // Nghỉ 3 giây để tránh bị quét
                await Task.Delay(3000, stoppingToken);
            }
        }

        private async Task SaveTaiLieuAI(Guid baiHocId, string content)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var baiHoc = await unitOfWork.BaiHocRepository.GetByIdAsync(baiHocId);
            if (baiHoc != null)
            {
                baiHoc.TaiLieuAI = content;
                unitOfWork.BaiHocRepository.Update(baiHoc);
                await unitOfWork.CompleteAsync(SystemUserId);
            }
        }

        private async Task UpdateStatusMessage(Guid baiHocId, string message)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var baiHoc = await unitOfWork.BaiHocRepository.GetByIdAsync(baiHocId);
            if (baiHoc != null)
            {
                baiHoc.TaiLieuAI = message;
                unitOfWork.BaiHocRepository.Update(baiHoc);
                await unitOfWork.CompleteAsync(SystemUserId);
            }
        }

        private string ExtractVideoId(string url)
        {
            if (url.Contains("youtu.be/"))
                return url.Split("youtu.be/").Last().Split('?').First();
            if (url.Contains("v="))
                return System.Web.HttpUtility.ParseQueryString(new Uri(url).Query)["v"];
            return url; // Đề phòng truyền thẳng ID vào
        }
    }
}
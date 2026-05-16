using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elearning.Publising.UI.Pages
{
    public class StudyModel : PageModel
    {
        private readonly ICallServiceRegistry _callService;

        public StudyModel(ICallServiceRegistry callService)
        {
            _callService = callService;
        }

        public KhoaHocDto KhoaHocDetail { get; set; } = new KhoaHocDto();

        // Chứa bài học nếu click vào Video
        public BaiHocDto ActiveLesson { get; set; } = new BaiHocDto();
        public string EmbedYoutubeUrl { get; set; } = string.Empty;

        // Chứa bộ câu hỏi nếu click vào Bài thi
        public BoCauHoiOnTapDto ActiveQuiz { get; set; } = new BoCauHoiOnTapDto();
        public bool IsQuizMode { get; set; } = false;

        // =======================================================
        // CÁC BIẾN QUẢN LÝ TIẾN ĐỘ HỌC TẬP
        // =======================================================
        public List<Guid> CompletedLessonIds { get; set; } = new List<Guid>();
        public int ProgressPercentage { get; set; } = 0;
        public int TotalItems { get; set; } = 0;
        public int CompletedItems { get; set; } = 0;

        // =======================================================
        // BỔ SUNG: CÁC BIẾN CHỨA LỊCH SỬ LÀM BÀI ÔN TẬP
        // =======================================================
        public class QuizHistoryDto
        {
            public Guid Id { get; set; }
            public float Diem { get; set; }
            public int SoCauDung { get; set; }
            public DateTime ThoiDiemBatDau { get; set; }
            public DateTime? ThoiDiemNop { get; set; }
        }
        public List<QuizHistoryDto> QuizHistory { get; set; } = new List<QuizHistoryDto>();

        public async Task<IActionResult> OnGetAsync(Guid courseId, Guid? lessonId, Guid? quizId)
        {
            if (courseId == Guid.Empty) return RedirectToPage("/Index");

            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // 1. Luôn phải lấy cấu trúc Khóa học để build Menu bên trái
            var reqCourse = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = $"/Publising/KhoaHoc/detail/{courseId}",
                HasAuthorization = false
            };
            var resCourse = await _callService.Get<KhoaHocDto>(reqCourse);
            if (resCourse.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && resCourse.Data != null)
            {
                KhoaHocDetail = resCourse.Data;
            }
            else return RedirectToPage("/Index");

            // 2. GỌI API LẤY DANH SÁCH BÀI HỌC ĐÃ HOÀN THÀNH
            if (!string.IsNullOrEmpty(userIdString))
            {
                var reqProgress = new ApiRequestModel
                {
                    ApiService = ServicesRegistryEnum.ServicePublising,
                    Endpoint = $"/Publising/TienDoHoc/khoa-hoc/{courseId}/user/{userIdString}",
                    HasAuthorization = false
                };

                var resProgress = await _callService.Get<List<Guid>>(reqProgress);
                if (resProgress.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && resProgress.Data != null)
                {
                    CompletedLessonIds = resProgress.Data;
                }
            }

            // TÍNH TOÁN % TIẾN ĐỘ THỂ HIỆN RA GIAO DIỆN
            TotalItems = KhoaHocDetail.ChuongHocs.SelectMany(c => c.BaiHocs).Count();
            CompletedItems = CompletedLessonIds.Count;
            if (TotalItems > 0)
            {
                ProgressPercentage = (int)Math.Round((double)CompletedItems / TotalItems * 100);
            }

            // 3. NẾU LÀ HỌC VIDEO
            if (lessonId.HasValue && lessonId.Value != Guid.Empty)
            {
                IsQuizMode = false;
                ActiveLesson = KhoaHocDetail.ChuongHocs
                    .SelectMany(c => c.BaiHocs)
                    .FirstOrDefault(b => b.Id == lessonId.Value) ?? new BaiHocDto();

                if (!string.IsNullOrEmpty(ActiveLesson.VideoUrl))
                {
                    EmbedYoutubeUrl = GetYoutubeEmbedUrl(ActiveLesson.VideoUrl);
                }
            }
            // 4. NẾU LÀ LÀM BÀI TEST
            else if (quizId.HasValue && quizId.Value != Guid.Empty)
            {
                IsQuizMode = true;

                // Lấy chi tiết bộ câu hỏi
                var reqQuiz = new ApiRequestModel
                {
                    ApiService = ServicesRegistryEnum.ServicePublising,
                    Endpoint = $"/Publising/BoCauHoiOnTap/{quizId.Value}",
                    HasAuthorization = false
                };
                var resQuiz = await _callService.Get<BoCauHoiOnTapDto>(reqQuiz);

                if (resQuiz.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && resQuiz.Data != null)
                {
                    ActiveQuiz = resQuiz.Data;
                }

                // =========================================================
                // BỔ SUNG: LẤY LỊCH SỬ LÀM BÀI NẾU ĐÃ ĐĂNG NHẬP
                // =========================================================
                if (!string.IsNullOrEmpty(userIdString))
                {
                    var reqHistory = new ApiRequestModel
                    {
                        ApiService = ServicesRegistryEnum.ServicePublising,
                        Endpoint = $"/Publising/BaiLam/history-quiz/{quizId.Value}/user/{userIdString}",
                        HasAuthorization = false
                    };
                    var resHistory = await _callService.Get<List<QuizHistoryDto>>(reqHistory);

                    if (resHistory.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && resHistory.Data != null)
                    {
                        // Sắp xếp lần nộp mới nhất lên đầu
                        QuizHistory = resHistory.Data.OrderByDescending(x => x.ThoiDiemNop).ToList();
                    }
                }
            }

            return Page();
        }

        // =========================================================
        // HÀM NHẬN REQUEST ĐÁNH DẤU HOÀN THÀNH TỪ GIAO DIỆN
        // =========================================================
        public class MarkCompleteRequest { public Guid BaiHocId { get; set; } }

        public async Task<IActionResult> OnPostMarkCompleteAsync([FromBody] MarkCompleteRequest payload)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return new JsonResult(new { success = false, message = "Vui lòng đăng nhập" });

            var apiRequest = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = "/Publising/TienDoHoc/mark-complete",
                HasAuthorization = false
            };

            var requestData = new { NguoiDungId = Guid.Parse(userIdString), BaiHocId = payload.BaiHocId };
            var response = await _callService.Post<bool>(apiRequest, requestData);

            if (response.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK)
            {
                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false, message = "Không thể lưu tiến độ." });
        }

        public async Task<IActionResult> OnPostSubmitQuizAsync([FromBody] NopBaiRequest payload)
        {
            if (payload == null || payload.BoCauHoiId == Guid.Empty)
                return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ" });

            // Lấy UserId từ Cookie đăng nhập hiện tại
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return new JsonResult(new { success = false, message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!" });
            }
            // Nhét vào gói hàng gửi đi
            payload.UserId = Guid.Parse(userIdString);

            var apiRequest = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = "/Publising/BoCauHoiOnTap/nop-bai",
                HasAuthorization = false
            };

            var response = await _callService.Post<float>(apiRequest, payload);

            if (response.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK)
            {
                return new JsonResult(new { success = true, diem = response.Data });
            }

            return new JsonResult(new { success = false, message = "Có lỗi xảy ra từ máy chủ khi nộp bài." });
        }

        public class ChatbotApiResponse
        {
            public bool success { get; set; }
            public string reply { get; set; }
        }

        public async Task<IActionResult> OnPostAskTutorAsync([FromBody] ChatbotRequestDto payload)
        {
            if (payload == null || string.IsNullOrWhiteSpace(payload.UserMessage))
                return new JsonResult(new { success = false, reply = "Bạn chưa nhập câu hỏi." });

            var apiRequest = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = "/Publising/Chatbot/ask-tutor",
                HasAuthorization = false
            };

            var response = await _callService.Post<ChatbotApiResponse>(apiRequest, payload);

            if (response.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && response.Data != null)
            {
                // Trả kết quả về lại cho Javascript
                return new JsonResult(new { success = response.Data.success, reply = response.Data.reply });
            }

            return new JsonResult(new { success = false, reply = "Xin lỗi, không thể kết nối đến não bộ AI lúc này." });
        }

        private string GetYoutubeEmbedUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;

            var videoIdMatch = Regex.Match(url, @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/\s]{11})");
            if (videoIdMatch.Success)
            {
                return $"https://www.youtube.com/embed/{videoIdMatch.Groups[1].Value}?rel=0&modestbranding=1&fs=1";
            }
            return url;
        }
    }
}
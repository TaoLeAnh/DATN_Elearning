using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Elearning.Publising.UI.Pages
{
    public class CourseDetailModel : PageModel
    {
        private readonly ICallServiceRegistry _callService;

        public CourseDetailModel(ICallServiceRegistry callService)
        {
            _callService = callService;
        }

        // Property để chứa dữ liệu khóa học truyền ra HTML
        public KhoaHocDto KhoaHocDetail { get; set; } = new KhoaHocDto();

        // Biến bắt lỗi (nếu có)
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return RedirectToPage("/Index");
            }

            var req = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = $"/Publising/KhoaHoc/detail/{id}",
            };

            var response = await _callService.Get<KhoaHocDto>(req);

            if (response.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && response.Data != null)
            {
                KhoaHocDetail = response.Data;
                return Page();
            }
            else
            {
                ErrorMessage = response.Message ?? "Lỗi tải dữ liệu";
                return Page();
            }
        }
        public async Task<IActionResult> OnPostRegisterAsync(Guid courseId)
        {
            // 1. KIỂM TRA ĐĂNG NHẬP NGAY TẠI UI
            if (!User.Identity.IsAuthenticated)
            {
                // Thông báo cho người dùng và đá về trang chi tiết khóa học (hoặc đá sang trang Login)
                TempData["ErrorMessage"] = "Bạn cần đăng nhập tài khoản để đăng ký khóa học này!";
                return RedirectToPage(new { id = courseId });

                // Hoặc nếu bác muốn đá thẳng sang trang đăng nhập thì dùng:
                // return Redirect("/Login"); 
            }

            if (courseId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Mã khóa học không hợp lệ.";
                return RedirectToPage(new { id = courseId });
            }
            var myToken = User.FindFirst("AccessToken")?.Value;
            // 2. GỌI API (Lúc này chắc chắn đã có Token/Cookie)
            var req = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = $"/Publising/DangKyKhoaHoc/register/{courseId}",
                HasAuthorization = true,
                Token = myToken
            };

            var response = await _callService.Post<dynamic>(req, new { });

            if (response.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK)
            {
                TempData["SuccessMessage"] = "Đăng ký thành công! Bạn có thể bắt đầu học.";
                return RedirectToPage(new { id = courseId });
            }
            else
            {
                string msg = response.Message;
                if (!string.IsNullOrWhiteSpace(msg) && msg.Trim().StartsWith("{"))
                {
                    try
                    {
                        using (var doc = JsonDocument.Parse(msg))
                        {
                            if (doc.RootElement.TryGetProperty("message", out var msgElement))
                            {
                                msg = msgElement.GetString();
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = $"Hệ thống đang bận (Mã lỗi: {response.Status}). Vui lòng thử lại sau.";
                }

                TempData["ErrorMessage"] = msg;
                return RedirectToPage(new { id = courseId });
            }
        }
    }
}

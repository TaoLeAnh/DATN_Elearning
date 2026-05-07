using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Elearning.Publising.UI.Pages
{
    public class LamBaiModel : PageModel
    {
        private readonly ICallServiceRegistry _callService;

        public LamBaiModel(ICallServiceRegistry callService)
        {
            _callService = callService;
        }

        public BoCauHoiOnTapDto ActiveQuiz { get; set; } = new BoCauHoiOnTapDto();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == Guid.Empty) return RedirectToPage("/Exam");

            var reqQuiz = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = $"/Publising/KyThi/{id}/lam-bai",
                HasAuthorization = false
            };

            var resQuiz = await _callService.Get<BoCauHoiOnTapDto>(reqQuiz);

            if (resQuiz.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && resQuiz.Data != null)
            {
                ActiveQuiz = resQuiz.Data;
                return Page();
            }

            return RedirectToPage("/Exam");
        }

        public async Task<IActionResult> OnPostSubmitQuizAsync([FromBody] NopBaiRequest payload)
        {
            if (payload == null || payload.BoCauHoiId == Guid.Empty)
                return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ" });

            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdString))
            {
                payload.UserId = Guid.Parse(userIdString);
            }

            var apiRequest = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = "/Publising/KyThi/nop-bai",
                HasAuthorization = false
            };

            // ĐÃ SỬA: Đổi <float> thành <dynamic>
            var response = await _callService.Post<dynamic>(apiRequest, payload);

            if (response.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK)
            {
                // ĐÃ SỬA: Trả thẳng cục JSON từ API về cho UI (chứa isLive, diem, message...)
                return new JsonResult(response.Data);
            }

            return new JsonResult(new { success = false, message = "Có lỗi xảy ra từ máy chủ khi nộp bài." });
        }

        // =========================================================================
        // BỔ SUNG: 2 API TRUNG GIAN CHO MODULE GIÁM THỊ
        // =========================================================================

        public class BatDauThiClientRequest { public Guid KyThiId { get; set; } }

        public async Task<IActionResult> OnPostBatDauThiAsync([FromBody] BatDauThiClientRequest req)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return new JsonResult(new { success = false, message = "Cần đăng nhập" });

            var apiRequest = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = "/Publising/KyThi/bat-dau-thi",
                HasAuthorization = false
            };

            var payload = new { KyThiId = req.KyThiId, UserId = Guid.Parse(userIdString) };
            var response = await _callService.Post<Guid>(apiRequest, payload);

            if (response.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK)
                return new JsonResult(new { success = true, baiLamId = response.Data });

            return new JsonResult(new { success = false, message = response.Message });
        }

        public class LogViPhamClientRequest
        {
            public Guid BaiLamId { get; set; }
            public EnumLoaiViPham LoaiViPham { get; set; }
            public string ChiTiet { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostLogViPhamAsync([FromBody] LogViPhamClientRequest req)
        {
            var apiRequest = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = "/Publising/KyThi/log-vi-pham",
                HasAuthorization = false
            };

            var response = await _callService.Post<bool>(apiRequest, req);
            return new JsonResult(new { success = response.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK });
        }
    }
}

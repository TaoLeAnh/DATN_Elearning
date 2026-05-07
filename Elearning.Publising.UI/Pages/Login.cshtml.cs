using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Elearning.Publising.UI.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ICallServiceRegistry _callService;

        public LoginModel(ICallServiceRegistry callService)
        {
            _callService = callService;
        }

        public class InputModel
        {
            public string Email { get; set; } = string.Empty;
            public string? Password { get; set; }
            public string? ConfirmPassword { get; set; }
            public string ActionType { get; set; } = "LOGIN";
        }

        // BỔ SUNG: Class để hứng chính xác UserId từ API Backend trả về
        public class AuthResponse
        {
            public bool Success { get; set; }
            public Guid UserId { get; set; }
            public string? VaiTro { get; set; }
            public string? Message { get; set; }
            public string? Token { get; set; }
        }

        public void OnGet() { }

        [HttpPost]
        public async Task<JsonResult> OnPostCheckEmailAsync([FromBody] CheckEmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Email)) return new JsonResult(new { success = false, message = "Email rỗng" });

            var apiReq = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = "/Publising/AuthPublising/check-email",
                HasAuthorization = false
            };

            var res = await _callService.Post<dynamic>(apiReq, request);

            if (res.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK)
            {
                return new JsonResult(res.Data);
            }
            return new JsonResult(new { success = false, message = "Lỗi kết nối máy chủ API (Check Email)" });
        }

        // HÀM 2: XỬ LÝ ĐĂNG NHẬP HOẶC ĐĂNG KÝ
        [HttpPost]
        public async Task<JsonResult> OnPostSubmitAuthAsync([FromBody] InputModel Input)
        {
            if (string.IsNullOrEmpty(Input.Email) || string.IsNullOrEmpty(Input.Password))
                return new JsonResult(new { success = false, message = "Vui lòng nhập đầy đủ thông tin!" });

            if (Input.ActionType == "LOGIN")
            {
                var loginReq = new LoginRequest { Email = Input.Email, Password = Input.Password };
                var apiReq = new ApiRequestModel
                {
                    ApiService = ServicesRegistryEnum.ServicePublising,
                    Endpoint = "/Publising/AuthPublising/login",
                    HasAuthorization = false
                };

                // ĐÃ SỬA: Dùng AuthResponse thay vì dynamic để bắt được UserId
                var res = await _callService.Post<AuthResponse>(apiReq, loginReq);

                if (res.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && res.Data != null)
                {
                    // LƯU Ý QUAN TRỌNG: Lưu UserId vào Cookie để IRequestContext lấy ra
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Input.Email.Split('@')[0]),
                        new Claim(ClaimTypes.Email, Input.Email),
                        
                        // Lưu ID theo chuẩn chung và cả tên "UserId" để bao quát mọi trường hợp
                        new Claim(ClaimTypes.NameIdentifier, res.Data.UserId.ToString()),
                        new Claim("UserId", res.Data.UserId.ToString()),
                         new Claim("AccessToken", res.Data.Token ?? "")
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Phát Cookie cho trình duyệt
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return new JsonResult(new { success = true, message = "Đăng nhập thành công!" });
                }
                return new JsonResult(new { success = false, message = "Email hoặc mật khẩu không chính xác." });
            }
            else if (Input.ActionType == "REGISTER")
            {
                var regReq = new RegisterRequest { Email = Input.Email, Password = Input.Password };
                var apiReq = new ApiRequestModel
                {
                    ApiService = ServicesRegistryEnum.ServicePublising,
                    Endpoint = "/Publising/AuthPublising/register",
                    HasAuthorization = false
                };

                // ĐÃ SỬA: Dùng AuthResponse thay vì dynamic
                var res = await _callService.Post<AuthResponse>(apiReq, regReq);

                if (res.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && res.Data != null)
                {
                    // ĐĂNG NHẬP LUÔN SAU KHI ĐĂNG KÝ
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Input.Email.Split('@')[0]),
                        new Claim(ClaimTypes.Email, Input.Email),
                        
                        // Lưu ID vào Cookie
                        new Claim(ClaimTypes.NameIdentifier, res.Data.UserId.ToString()),
                        new Claim("UserId", res.Data.UserId.ToString()),
                        new Claim("AccessToken", res.Data.Token ?? "")
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return new JsonResult(new { success = true, message = "Tạo tài khoản và đăng nhập thành công!" });
                }
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra. Email này có thể đã tồn tại." });
            }

            return new JsonResult(new { success = false, message = "Yêu cầu không hợp lệ." });
        }
    }
}
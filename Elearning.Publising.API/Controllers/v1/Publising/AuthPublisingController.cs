using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Elearning.Publising.API.Controllers.v1.Publising
{
    [ApiController]
    [Route("api/v1/Publising/[controller]")]
    public class AuthPublisingController : ControllerBase
    {
        private readonly IAuthPublisingService _authService;
        private readonly IConfiguration _configuration; // Thêm thằng này để đọc cấu hình

        // Tiêm IConfiguration vào Constructor
        public AuthPublisingController(IAuthPublisingService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("check-email")]
        public async Task<IActionResult> CheckEmail([FromBody] CheckEmailRequest request)
        {
            bool isExist = await _authService.CheckEmailExistAsync(request.Email);
            return Ok(new { success = true, exists = isExist });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await _authService.RegisterAsync(request);

            // Dùng Ok kèm success = false để UI dùng CallServiceRegistry dễ bắt lỗi hơn
            if (user == null)
                return Ok(new { success = false, message = "Email đã tồn tại!" });

            // 1. IN TOKEN NGAY KHI ĐĂNG KÝ XONG
            var tokenString = GenerateJwtToken(user.Id, user.Email, user.VaiTro.ToString());

            // 2. TRẢ TOKEN VỀ CHO UI HỨNG
            return Ok(new
            {
                success = true,
                userId = user.Id,
                vaiTro = user.VaiTro.ToString(),
                token = tokenString, // <--- ĐIỂM MẤU CHỐT LÀ ĐÂY
                message = "Đăng ký thành công"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.LoginAsync(request);

            if (user == null)
                return Ok(new { success = false, message = "Email hoặc mật khẩu không đúng!" });

            // 1. IN TOKEN KHI ĐĂNG NHẬP THÀNH CÔNG
            var tokenString = GenerateJwtToken(user.Id, user.Email, user.VaiTro.ToString());

            // 2. TRẢ TOKEN VỀ CHO UI HỨNG
            return Ok(new
            {
                success = true,
                userId = user.Id,
                vaiTro = user.VaiTro.ToString(),
                token = tokenString // <--- ĐIỂM MẤU CHỐT LÀ ĐÂY
            });
        }

        // =======================================================
        // HÀM BÍ MẬT: CHUYÊN MÁY IN TOKEN
        // =======================================================
        private string GenerateJwtToken(Guid userId, string email, string role)
        {
            // Đọc Key từ appsettings.json, nếu không có thì dùng Key mặc định này (Phải giống file Program.cs)
            var secretKey = _configuration["JwtSettings:SecretKey"] ?? "MotChuoiBiMatSieuDaiVaAnToanChoElearningProjectCuaTao";
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            // Nhét thông tin (Claims) vào cái Token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(30), // Cấp hạn dùng 30 ngày
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Trả về chuỗi Token hoàn chỉnh
            return tokenHandler.WriteToken(token);
        }
    }
}
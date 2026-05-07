using Elearning.Application.Interfaces;
using Elearning.Shared.Commons.Model.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Elearning.API.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequest form)
        {
            if (string.IsNullOrWhiteSpace(form.Email) || string.IsNullOrWhiteSpace(form.Password))
                return BadRequest("Email và mật khẩu không được để trống.");

            var user = await _authService.LoginAsync(form.Email, form.Password);

            if (user == null)
                return Unauthorized("Email hoặc mật khẩu không chính xác.");

            var secretKey = _configuration["JwtSettings:SecretKey"];
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.VaiTro.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Token sống 7 ngày
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);

            // Trả về DTO siêu nhẹ nhàng
            var userDto = new AuthResponseDto
            {
                Id = user.Id,
                Ten = user.Ten,
                Email = user.Email,
                VaiTro = user.VaiTro.ToString(), // Ép sang chuỗi cho dễ đọc
                Token = jwtString
            };

            return Ok(userDto);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // --- CLASS MỚI ĐỂ TRẢ VỀ CHO FRONTEND ---
    public class AuthResponseDto
    {
        public Guid Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
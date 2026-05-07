using Elearning.Application.Interfaces;
using Elearning.Application.Services;
using Elearning.Domain.Interfaces;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.UnitOfWorks;
using Elearning.Infrastructure.Security;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// 1. CẤU HÌNH CONTROLLER
builder.Services.AddControllers();

// 2. CẤU HÌNH DATABASE
builder.Services.AddDbContext<ElearningDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. CẤU HÌNH CORS (Cho phép UI gọi API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. ĐĂNG KÝ CÁC DỊCH VỤ (DEPENDENCY INJECTION)

// 4.1 Hỗ trợ lấy thông tin request/user
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRequestContext, RequestContext>();

// 4.2 Tầng Infrastructure
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 4.3 Tầng Application Services
builder.Services.AddScoped<IKhoaHocService, KhoaHocService>();
builder.Services.AddScoped<IChuongHocService, ChuongHocService>();
builder.Services.AddScoped<IBaiHocService, BaiHocService>();
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<IDangKyKhoaHocService, DangKyKhoaHocService>();
builder.Services.AddScoped<ITienDoHocService, TienDoHocService>();
builder.Services.AddScoped<IBoCauHoiOnTapService, BoCauHoiOnTapService>();
builder.Services.AddScoped<ICauHoiService, CauHoiService>();
builder.Services.AddScoped<IKyThiService, KyThiService>();
builder.Services.AddScoped<IBaiLamService, BaiLamService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILogViPhamService, LogViPhamService>();
builder.Services.AddScoped<IMaTranDeThiMacDinhService, MaTranDeThiMacDinhService>();
builder.Services.AddScoped<IHoSoGiaoVienService, HoSoGiaoVienService>();

builder.Services.AddScoped<IStorageService, MinioStorageService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

var secretKey = builder.Configuration["JwtSettings:SecretKey"];
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });
builder.Services.AddAuthorization();

// Thêm OpenAPI (Swagger)
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
// Bật CORS đã cấu hình ở trên
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// =========================================================================
// BẮT ĐẦU CHÈN THÊM ĐOẠN MIDDLEWARE NÀY
// Nhiệm vụ: Bắt lấy User ID từ Token (JWT) và nhét vào RequestContext
// =========================================================================
app.Use(async (context, next) =>
{
    // Kiểm tra xem request này có mang theo Token hợp lệ không
    if (context.User.Identity?.IsAuthenticated == true)
    {
        // Lấy hộp RequestContext ra
        var requestContext = context.RequestServices.GetRequiredService<IRequestContext>();

        // Trích xuất ID từ JWT Claim. Tùy thuộc vào lúc tạo Token ở AuthService bạn dùng key gì, 
        // thường nó sẽ là ClaimTypes.NameIdentifier hoặc "nameid"
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                       ?? context.User.FindFirst("nameid")?.Value;

        if (Guid.TryParse(userIdClaim, out Guid userId))
        {
            // Nhét dữ liệu vào RequestContext để UnitOfWork / DbContext sử dụng
            requestContext.CurrentUser = new Elearning.Shared.Commons.Model.Commons.CurrentUserDto
            {
                UserId = userId
            };
            requestContext.IsUser = true;
        }
    }

    // Cho phép Request đi tiếp tới các Controller (CauHoiController,...)
    await next();
});
// =========================================================================
// KẾT THÚC ĐOẠN CHÈN THÊM
// =========================================================================

app.MapControllers();
app.Run();
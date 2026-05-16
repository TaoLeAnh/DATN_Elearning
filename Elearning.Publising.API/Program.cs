using Elearning.Infrastructure.Security;
using Elearning.Publising.Application;
using Elearning.Publising.Infrastructure;
using Elearning.Shared.Commons.Extensions;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new LocalDateTimeConverter());
    });

// Đăng ký tầng Application & Infrastructure
builder.Services.AddServicesDependencies(builder.Configuration);
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddHttpClient<IAIService, AIService>();
builder.Services.AddInfrustructureDependencies(builder.Configuration);

// ================================================================
// 2. THÊM CẤU HÌNH JWT BẢO VỆ API 
// ================================================================
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
// ================================================================

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ================================================================
// 3. THÊM XÁC THỰC VÀO PIPELINE (Phải đặt trước UseAuthorization)
// ================================================================
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
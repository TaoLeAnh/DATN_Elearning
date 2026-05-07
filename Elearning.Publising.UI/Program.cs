using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Commons.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Đăng ký dịch vụ ICallServiceRegistry để các trang có thể gọi được API
builder.Services.AddHttpClient<ICallServiceRegistry, CallServiceRegistryPublishing>();

// ===========================================================
// CHỈ GIỮ LẠI ĐÚNG 1 CỤM KHAI BÁO COOKIE NÀY THÔI BÁC NHÉ
// ===========================================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Đường dẫn tới trang đăng nhập của bác
        options.AccessDeniedPath = "/AccessDenied"; // Đường dẫn khi không có quyền
        options.ExpireTimeSpan = TimeSpan.FromDays(1); // Giữ đăng nhập 30 ngày
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Bắt buộc phải có 2 dòng này để Razor Page biết ai đang đăng nhập
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
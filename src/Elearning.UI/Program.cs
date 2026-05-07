using Elearning.UI.Application;
using Elearning.UI.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Services registration
builder.Services.AddFluentUIComponents();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// --- AUTHENTICATION CONFIG ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Elearning.Portal.Auth";
        options.LoginPath = "/account/login";
        options.LogoutPath = "/account/logout";
        options.AccessDeniedPath = "/account/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// --- ANTIFORGERY CONFIG ---
builder.Services.AddAntiforgery();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// --- APP SERVICES ---
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<Elearning.Shared.Commons.Interfaces.Extentions.ICallServiceRegistry, Elearning.UI.Application.CallServiceRegistryCMS>();
builder.Services.AddSingleton<Elearning.Shared.Commons.Interfaces.Extentions.ICacheService, SimpleCacheService>();
builder.Services.AddSingleton<Elearning.Shared.Commons.Interfaces.Extentions.IRequestContext, NoOpRequestContext>();
builder.Services.AddScoped<Elearning.UI.Application.IModuleTypeResolver, Elearning.UI.Application.DefaultModuleTypeResolver>();
builder.Services.AddScoped<ModuleTypeState>();
builder.Services.AddSingleton<Elearning.UI.Application.IModuleRegistry>(sp =>
    new Elearning.UI.Application.ModuleRegistry(
        new Elearning.UI.Application.INavModule[] {
            new Elearning.UI.Application.AdminModule(),
            new Elearning.UI.Application.NghiepVuModule()
        }
    ));

builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

// --- MIDDLEWARE PIPELINE ---
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// --- MINIMAL API: SET COOKIE SAU KHI BLAZOR GỌI API LOGIN THÀNH CÔNG ---
app.MapGet("/account/do-login", async (
    HttpContext ctx,
    string id,
    string ten,
    string email,
    string vaitro,
    string token, // <--- 1. THÊM THAM SỐ NÀY ĐỂ HỨNG
    string? returnUrl) =>
{
    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(vaitro))
        return Results.Redirect("/account/login");

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, id),
        new Claim(ClaimTypes.Name, ten ?? ""),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, vaitro),
        
        // <--- 2. THÊM DÒNG NÀY: Cất Token vào két sắt Cookie
        new Claim("AccessToken", token)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    var safeReturn = !string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/")
        ? returnUrl
        : "/nghiep-vu/quan-ly-ky-thi";

    return Results.Redirect(safeReturn);
})
.AllowAnonymous();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
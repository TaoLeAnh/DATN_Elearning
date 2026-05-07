using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.Extentions.Redis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Elearning.UI.Application
{
    public interface IUserService
    {
        Task<CurrentUserDto> GetCurrentUserAsync();
        Task<string> GetTokenCurrentUserAsync();
        Task SignOutUserAsync();
    }
    public class UserService : IUserService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IRequestContext _requestContext;
        private readonly ICacheService _cacheService;
        private readonly NavigationManager _navigationManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            AuthenticationStateProvider authenticationStateProvider,
            IRequestContext requestContext,
            ICacheService cacheService,
            NavigationManager navigationManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _requestContext = requestContext;
            _cacheService = cacheService;
            _navigationManager = navigationManager;
            _httpContextAccessor = httpContextAccessor;
        }

        //public async Task<CurrentUserDto> GetCurrentUserAsync()
        //{
        //    try
        //    {
        //        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        //        var user = authState.User;


        //        if (user.Identity?.IsAuthenticated != true)
        //            return new CurrentUserDto();

        //        string email = user.FindFirst("email")?.Value ?? string.Empty;
        //        string userName = user.FindFirst("preferred_username")?.Value ?? string.Empty;
        //        string token = user.FindFirst("preferred_token")?.Value ?? string.Empty;

        //        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(token))
        //            return await SignOutAndReturnEmptyUser();

        //        var userId = GetUserIdFromToken(token);
        //        if (string.IsNullOrEmpty(userId))
        //            return await SignOutAndReturnEmptyUser();


        //        string cacheKey = $"{userId}-session-active";


        //        var currentUser = await _cacheService.GetAsync<CurrentUserDto>(RedisTypeKey.Session, cacheKey);

        //        if (currentUser is null)
        //        {
        //            return await SignOutAndReturnEmptyUser();
        //        }


        //        return currentUser;
        //    }
        //    catch
        //    {
        //        return await SignOutAndReturnEmptyUser();
        //    }
        //}
        public async Task<CurrentUserDto> GetCurrentUserAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated != true)
                return null;

            // Map dữ liệu từ Cookie vào đúng các trường của CurrentUserDto cũ
            return new CurrentUserDto
            {
                UserId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString()),
                FullName = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty, // Ánh xạ Name thành FullName
                Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                UserName = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                SupperUser = user.FindFirst(ClaimTypes.Role)?.Value == "Admin" // Admin thì là SupperUser
            };
        }
        ///// <summary>
        ///// CongVM
        ///// Gets the access token of the currently authenticated user
        ///// </summary>
        ///// <returns>The access token string or empty string if user is not authenticated</returns>
        //public async Task<string> GetTokenCurrentUserAsync()
        //{
        //    var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        //    var user = authState.User;

        //    if (user?.Identity?.IsAuthenticated != true)
        //        return string.Empty;

        //    return user.FindFirst("preferred_token")?.Value ?? string.Empty;
        //}
        /// <summary>
        /// CongVM
        /// Gets the access token of the currently authenticated user
        /// </summary>
        /// <returns>The access token string or empty string if user is not authenticated</returns>
        public async Task<string> GetTokenCurrentUserAsync()
        {
            // --- THÊM DÒNG NÀY ---
            // Đợi 100ms để Blazor cập nhật xong trạng thái Auth từ Cookie vào bộ nhớ
            await Task.Delay(100);

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user?.Identity?.IsAuthenticated != true)
                return string.Empty;

            // Xóa đoạn Console.WriteLine đi cho đỡ rối mắt
            return user.FindFirst("AccessToken")?.Value ?? string.Empty;
        }

        private async Task<CurrentUserDto> SignOutAndReturnEmptyUser()
        {
            await SignOutUserAsync();
            return new CurrentUserDto();
        }


        private string GetUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return string.Empty;

            try
            {
                var jwtHand = new JwtSecurityTokenHandler();
                var securityToken = jwtHand.ReadJwtToken(token);
                var userIdClaim = securityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti);
                if (userIdClaim is null)
                {
                    return string.Empty;
                }
                return userIdClaim.Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task SignOutUserAsync()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                // For server-side Blazor or ASP.NET Core MVC
                if (context is not null && context.User is not null && context.User.Identity is not null && context.User.Identity.IsAuthenticated)
                {
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    //await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                }
            }
            catch
            {


            }


            // Redirect to login page after sign-out
            _navigationManager.NavigateTo("account/login", forceLoad: true);
        }


    }
}

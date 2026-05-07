using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.Extentions.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public class JwtMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly RequestDelegate _next;
        private readonly ICacheService _redisCache;
        public JwtMiddleware(RequestDelegate next, ICacheService redisCache, IConfiguration configuration)
        {
            _next = next;
            _redisCache = redisCache;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, IRequestContext requestContext)
        {

            string? token = context.Request.Headers["Authorization"].FirstOrDefault();
            var MinuteExpireToken = _configuration["JwtSettings:MinuteExpireToken"] ?? throw new ArgumentNullException("MinuteExpireToken not found!");
            string? clientHashClaim = context.User.FindFirst("ClientHash")?.Value;
            try
            {

                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = token.Substring("Bearer ".Length).Trim();

                    // 1. Giải mã token
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

                    JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

                    bool valid = IsTokenExpired(jwtToken, out string sessionId);

                    if (string.IsNullOrEmpty(sessionId))
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }

                    // Kiểm tra token có phải là active token của user không
                    var activeTokenKey = $"{sessionId}-token-active";
                    var activeSessionKey = $"{sessionId}-session-active";



                    // Kiểm tra token active
                    var activeTokenExists = await _redisCache.KeyExistsAsync(RedisTypeKey.Session, activeTokenKey);
                    if (!activeTokenExists)
                    {

                        context.Response.StatusCode = 401;
                        return;
                    }


                    CurrentUserDto infoUser = await _redisCache.GetAsync<CurrentUserDto>(RedisTypeKey.Session, activeSessionKey) ?? new();
                    if (infoUser == null || infoUser.UserId.IsEmpty())
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Token not valid");
                        return;
                    }

                    if (string.IsNullOrEmpty(infoUser.Token))
                    {
                        infoUser.Token = token;
                        await _redisCache.SetAsync(RedisTypeKey.Session, activeSessionKey, infoUser, TimeSpan.FromMinutes(int.Parse(MinuteExpireToken)));
                    }


                    requestContext.CurrentUser = infoUser;
                    requestContext.IsUser = true;
                }
                else if (!string.IsNullOrEmpty(clientHashClaim))
                {
                    // Tìm theo ClientId
                    OAuthClients? OAuthClientsFound = await _redisCache.HashGetAsync<OAuthClients>(
                        RedisTypeKey.Core,
                        "OAuthClients",
                        clientHashClaim
                    );

                    if (OAuthClientsFound == null || OAuthClientsFound.ClientId.IsEmpty())
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("ClientId || ClientSecret not valid");
                        return;
                    }

                    requestContext.CurrentClients = OAuthClientsFound;
                    requestContext.IsUser = false;

                }


            }
            catch
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }


            await _next(context);
        }

        private bool IsTokenExpired(JwtSecurityToken jwtToken, out string jti)
        {
            jti = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(jti))
            {
                return true;
            }
            // 2. Lấy claim expiration time (exp)
            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");

            if (expClaim == null)
            {
                return true; // Token không có claim hết hạn (bất thường)
            }

            // 3. Chuyển giá trị exp thành DateTimeOffset
            if (!long.TryParse(expClaim.Value, out long expValue))
            {
                return true; // Giá trị exp không hợp lệ
            }
            DateTimeOffset expirationTime = DateTimeOffset.FromUnixTimeSeconds(expValue);

            // 4. Kiểm tra hết hạn
            return expirationTime <= DateTimeOffset.UtcNow;
        }
    }
}

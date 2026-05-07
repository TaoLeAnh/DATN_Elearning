using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public class TokenHelper
    {
        public static bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                return true; // Token không hợp lệ
            }

            DateTime expiryDate = jwtToken.ValidTo; // Thời gian hết hạn của token (UTC)
            return expiryDate < DateTime.UtcNow; // Kiểm tra nếu đã hết hạn
        }

        public static string? GetClaimValue(string token, string claimType)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                return null; // Token không hợp lệ
            }

            // Lấy giá trị của claim theo tên
            return jwtToken.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}

using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Services
{
    public class RequestContext : IRequestContext
    {
        private CurrentUserDto _currentUser = new CurrentUserDto();
        public CurrentUserDto CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value ?? throw new ArgumentNullException(nameof(value));
        }

        private OAuthClients _currentClients = new OAuthClients();
        public OAuthClients CurrentClients
        {
            get => _currentClients;
            set => _currentClients = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// True => dùng user JWT,
        /// False => dùng Client credentials
        /// </summary>
        private bool _isUser = true;
        public bool IsUser
        {
            get => _isUser;
            set => _isUser = value;
        }

        /// <summary>
        /// Nếu IsUser==true: trả về CurrentUser.UserId
        /// Nếu IsUser==false: trả về CurrentClients.ClientId (hoặc Id tuỳ model)
        /// </summary>
        public Guid CurrentIdUser
            => IsUser
                ? CurrentUser.UserId
                : CurrentClients.ClientId;    // hoặc .Id tuỳ model của bạn

        /// <summary>
        /// Nếu IsUser==true: trả về CurrentUser.Token
        /// Nếu IsUser==false: trả về string.Empty
        /// </summary>
        public string CurrentTokenUser
            => IsUser
                ? (CurrentUser.Token ?? string.Empty)
                : string.Empty;

        /// <summary>
        /// Địa chỉ IP khách
        /// </summary>
        private string _ipAddress = string.Empty;
        public string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }
    }
}

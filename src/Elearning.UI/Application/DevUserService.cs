using System.Threading.Tasks;
using Elearning.Shared.Commons.Model.Commons;
using System.Collections.Generic;

namespace Elearning.UI.Application
{
    // Development-only user service that returns a fake CurrentUser for UI testing
    public class DevUserService : IUserService
    {
        private readonly CurrentUserDto _devUser = new CurrentUserDto
        {
            UserName = "dev.user",
            FullName = "Developer",
            SupperUser = true,
            Menus = new List<MenuItemDto>()
        };

        public Task<CurrentUserDto> GetCurrentUserAsync()
        {
            return Task.FromResult(_devUser);
        }

        public Task<string> GetTokenCurrentUserAsync()
        {
            return Task.FromResult(string.Empty);
        }

        public Task SignOutUserAsync()
        {
            return Task.CompletedTask;
        }
    }
}

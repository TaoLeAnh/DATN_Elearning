using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons;

namespace Elearning.UI.Application
{
    public class NoOpRequestContext : IRequestContext
    {
        public CurrentUserDto CurrentUser { get; set; } = new CurrentUserDto();
        public OAuthClients CurrentClients { get; set; } = new OAuthClients();
        public bool IsUser { get; set; } = false;
        public Guid CurrentIdUser => Guid.Empty;
        public string CurrentTokenUser => string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
}

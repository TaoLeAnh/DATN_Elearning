using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.Commons
{
    public class CurrentTokenDto
    {
        public string IpAddress { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public Guid ClientId { get; set; }
    }
}

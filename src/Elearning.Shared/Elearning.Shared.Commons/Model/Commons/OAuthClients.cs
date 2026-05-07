using Elearning.Shared.Commons.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.Commons
{
    public class OAuthClients
    {
        /// <summary>
        /// Id group mà client thuộc về
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// Id của client trên DB
        /// </summary>
        public Guid ClientId { get; set; }
        public string MaClient { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;

        public string ClientHash
        {
            get
            {
                return CryptoHelper.Sha256Hash($"{MaClient}:{ClientSecret}");
            }
        }
    }
}

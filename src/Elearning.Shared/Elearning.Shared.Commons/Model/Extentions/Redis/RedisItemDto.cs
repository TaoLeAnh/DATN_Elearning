using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.Extentions.Redis
{
    public class RedisItemDto
    {
        public RedisTypeKey TypeKey { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public TimeSpan? TTL { get; set; }
    }
}

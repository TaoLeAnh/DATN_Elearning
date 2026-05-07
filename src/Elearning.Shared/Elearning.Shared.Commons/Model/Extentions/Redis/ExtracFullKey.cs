using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.Extentions.Redis
{
    /// <summary>
    /// Tách ky thành key con, 
    /// </summary>
    public class ExtracFullKey
    {
        public string NonePreFixKey { get; set; } = string.Empty;
        public RedisTypeKey HeadNonePrefixKey { get; set; }
        public string BodyNoneFrefixKey { get; set; } = string.Empty;
    }
}

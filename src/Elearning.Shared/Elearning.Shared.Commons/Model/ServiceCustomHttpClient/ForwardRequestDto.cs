using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Elearning.Shared.Commons.Model.ServiceCustomHttpClient
{
    public class ForwardRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public Dictionary<string, string> RouteParams { get; set; } = new();
        public Dictionary<string, string> QueryParams { get; set; } = new();
        public Dictionary<string, string> Headers { get; set; } = new();
        public byte[] Body { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
    }
    public class ForwardRequestToLogDto
    {
        public string Code { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public Dictionary<string, string> RouteParams { get; set; } = new();
        public Dictionary<string, string> QueryParams { get; set; } = new();
        public Dictionary<string, string> Headers { get; set; } = new();
        public JsonElement Body { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}

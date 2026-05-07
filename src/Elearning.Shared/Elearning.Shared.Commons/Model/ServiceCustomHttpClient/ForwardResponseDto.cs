using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Elearning.Shared.Commons.Model.ServiceCustomHttpClient
{
    public class ForwardResponseDto
    {
        public int StatusCode { get; set; }
        public byte[] Content { get; set; } = new byte[0];
        public string ContentTxt { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;
        public Dictionary<string, string[]> Headers { get; set; } = new();
        public int RequestTime { get; set; }
    }
    public class ForwardResponseToLogDto
    {
        public int StatusCode { get; set; }
        public JsonElement Content { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public Dictionary<string, string[]> Headers { get; set; } = new();
    }
}

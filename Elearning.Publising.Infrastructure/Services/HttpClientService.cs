using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;

namespace Elearning.Publising.Infrastructure.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpClientService> _logger;

        public HttpClientService(IHttpClientFactory httpClientFactory, ILogger<HttpClientService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ForwardResponseDto> SendAsync(string url, ForwardRequestDto request)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromMinutes(2);

            var requestMessage = new HttpRequestMessage
            {
                Method = new HttpMethod(request.Method),
                RequestUri = new Uri(url)
            };

            // Copy headers
            foreach (var header in request.Headers)
            {
                if (!IsRestrictedHeader(header.Key))
                {
                    requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Set content
            if (request.Body?.Length > 0)
            {
                requestMessage.Content = CreateContent(request.Body, request.ContentType);
            }

            _logger.LogInformation("Forwarding {Method} request to {Url}", request.Method, url);

            var response = await httpClient.SendAsync(requestMessage);

            return await CreateResponseDto(response);
        }

        private HttpContent CreateContent(byte[] body, string contentType)
        {
            HttpContent content = new ByteArrayContent(body);
            if (!string.IsNullOrEmpty(contentType))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }
            return content;
        }

        private async Task<ForwardResponseDto> CreateResponseDto(HttpResponseMessage response)
        {
            // 1) Đọc raw bytes (vẫn nén)
            var raw = await response.Content.ReadAsByteArrayAsync();

            // 2) Xem header để biết encoding
            var encs = response.Content.Headers.ContentEncoding;

            // 3) Giải nén nếu cần
            var processed = DecompressIfNeeded(raw, encs);

            // 4) Lấy charset (mặc định UTF-8) và decode thành text
            var charset = response.Content.Headers.ContentType?.CharSet ?? "UTF-8";
            var text = Encoding.GetEncoding(charset).GetString(processed);



            var headers = new Dictionary<string, string[]>();

            foreach (var header in response.Headers.Concat(response.Content.Headers))
            {
                if (!IsRestrictedResponseHeader(header.Key))
                {
                    headers[header.Key] = header.Value.ToArray();
                }
            }

            return new ForwardResponseDto
            {
                StatusCode = (int)response.StatusCode,
                Content = raw,
                ContentTxt = text,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream",
                Headers = headers
            };
        }
        // Hàm kiểm tra và giải nén nếu cần
        byte[] DecompressIfNeeded(byte[] raw, IEnumerable<string> encodings)
        {
            Stream stream = new MemoryStream(raw);

            if (encodings.Contains("br"))
                stream = new BrotliStream(stream, CompressionMode.Decompress);
            else if (encodings.Contains("gzip"))
                stream = new GZipStream(stream, CompressionMode.Decompress);
            else if (encodings.Contains("deflate"))
                stream = new DeflateStream(stream, CompressionMode.Decompress);

            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
        private static bool IsRestrictedHeader(string headerName)
        {
            var restrictedHeaders = new[]
            {
            "Host", "Content-Length", "Transfer-Encoding", "Connection",
            "Upgrade", "Proxy-Connection", "Proxy-Authenticate", "Proxy-Authorization",
            //"ClientID", "ClientSecret"
        };

            return restrictedHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsRestrictedResponseHeader(string headerName)
        {
            var restrictedHeaders = new[]
            {
            "Transfer-Encoding", "Connection", "Upgrade", "Server"
        };

            return restrictedHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase);
        }
    }
}

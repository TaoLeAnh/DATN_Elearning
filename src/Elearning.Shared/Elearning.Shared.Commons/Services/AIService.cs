using Elearning.Shared.Commons.Interfaces.Extentions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Elearning.Shared.Commons.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AIService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config.GetValue<string>("AIConfiguration:GeminiApiKey");
        }

        public async Task<string> ChatWithGeminiAsync(string prompt)
        {
            try
            {
                // 👉 ĐỔI TÊN MODEL: Dùng gemini-1.5-flash-latest hoặc gemini-pro để tránh lỗi 404

                var url =
  $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash-lite:generateContent?key={_apiKey}";
                // Định dạng body chuẩn của Google Gemini
                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                // 👉 BẪY LỖI XỊN: Nếu Google báo lỗi (400, 403, 404), trả thẳng lỗi đó về UI để dễ debug
                if (!response.IsSuccessStatusCode)
                {
                    return $"Google API Error ({response.StatusCode}): {responseString}";
                }

                var jsonDocument = JsonDocument.Parse(responseString);

                // Bóc tách câu trả lời từ Json của Google
                var answer = jsonDocument.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                return answer ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"Lỗi hệ thống khi gọi AI: {ex.Message}";
            }
        }
    }
}

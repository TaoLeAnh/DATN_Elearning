using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Commons.Services;
using System.Buffers;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Elearning.UI.Application
{
    public class CallServiceRegistryCMS : ICallServiceRegistry
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CallServiceRegistryAPI> _logger;

        // ✅ Reuse JsonSerializerOptions
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // ✅ Performance monitoring
        private readonly TimeSpan _threshold;
        private const int MaxResponseSizeBytes = 10 * 1024 * 1024; // 10MB
        private const int LohThreshold = 85_000; // 85KB

        // ✅ Token caching
        private string? _cachedToken;
        private bool _tokenInitialized = false;

        public CallServiceRegistryCMS(
            HttpClient httpClient,
            IConfiguration configuration,
            IUserService userService,
            ILogger<CallServiceRegistryAPI> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _userService = userService;
            _logger = logger;

            var thresholdMs = configuration.GetValue<int>("PerformanceMonitoring:ThresholdMs", 500);
            _threshold = TimeSpan.FromMilliseconds(thresholdMs);
        }

        private async Task<T> ExecuteWithLogging<T>(
            string httpMethod,
            ApiRequestModel apiRequestModel,
            Func<string, Task<T>> action)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];

            await EnsureAuthorizationAsync(apiRequestModel);
            var endpoint = GetFullEndPoint(apiRequestModel);

            try
            {
                var result = await action(endpoint);
                stopwatch.Stop();

                if (stopwatch.Elapsed >= _threshold)
                {
                    _logger.LogWarning(
                        "[{RequestId}] [{Method}] SLOW: {Endpoint} | {Duration}ms | Service: {Service}",
                        requestId, httpMethod, endpoint, stopwatch.ElapsedMilliseconds, apiRequestModel.ApiService);
                }
                else
                {
                    _logger.LogInformation(
                        "[{RequestId}] [{Method}] OK: {Endpoint} | {Duration}ms",
                        requestId, httpMethod, endpoint, stopwatch.ElapsedMilliseconds);
                }

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "[{RequestId}] [{Method}] ERROR: {Endpoint} | {Duration}ms | {Error}",
                    requestId, httpMethod, endpoint, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        public async Task<ResultAPI> Delete(ApiRequestModel apiRequestModel)
        {
            return await ExecuteWithLogging("DELETE", apiRequestModel, async (endpoint) =>
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                return await HandleResponse(response, endpoint);
            });
        }

        public async Task<ResultAPI> Put(ApiRequestModel apiRequestModel, object? data = null)
        {
            return await ExecuteWithLogging("PUT", apiRequestModel, async (endpoint) =>
            {
                var response = data == null
                    ? await _httpClient.PutAsync(endpoint, null)
                    : await _httpClient.PutAsJsonAsync(endpoint, data);
                return await HandleResponse(response, endpoint);
            });
        }

        public async Task<ResultAPI<T>> Get<T>(ApiRequestModel apiRequestModel)
        {
            return await ExecuteWithLogging("GET", apiRequestModel, async (endpoint) =>
            {
                var response = await _httpClient.GetAsync(endpoint);
                return await HandleResponse<T>(response, endpoint);
            });
        }

        public async Task<ResultAPI<T>> Post<T>(ApiRequestModel apiRequestModel, object data)
        {
            return await ExecuteWithLogging("POST", apiRequestModel, async (endpoint) =>
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                return await HandleResponse<T>(response, endpoint);
            });
        }

        public async Task<ResultAPI<byte[]>> PostForFile(ApiRequestModel apiRequestModel, object data)
        {
            return await ExecuteWithLogging("POST_FILE", apiRequestModel, async (endpoint) =>
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                return await HandleFileResponse(response, endpoint);
            });
        }

        private string GetFullEndPoint(ApiRequestModel apiRequestModel)
        {
            if (_configuration == null)
                throw new InvalidOperationException("Configuration is not provided.");

            string serviceBaseUrl = apiRequestModel.ApiService == ServicesRegistryEnum.CustomApi
                ? apiRequestModel.ApiServiceCustom ?? throw new KeyNotFoundException("ApiServiceCustom invalid.")
                : _configuration[$"ServicesRegistry:{apiRequestModel.ApiService}"]
                  ?? throw new KeyNotFoundException($"ServicesRegistry {apiRequestModel.ApiService} not found.");

            var fullPath = $"{serviceBaseUrl}/api{apiRequestModel.Endpoint}";

            if (apiRequestModel.QueryParams?.Any() == true)
            {
                var queryString = string.Join("&",
                    apiRequestModel.QueryParams.Select(q => $"{q.Key}={Uri.EscapeDataString(q.Value)}"));
                fullPath = $"{fullPath}?{queryString}";
            }

            return fullPath;
        }

        private async Task EnsureAuthorizationAsync(ApiRequestModel apiRequestModel)
        {
            if (!apiRequestModel.HasAuthorization)
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return;
            }

            string? token;

            // Token ngoài hệ thống → dùng trực tiếp, không cache
            if (!string.IsNullOrEmpty(apiRequestModel.Token))
            {
                token = await GetTokenFromRequestAsync(apiRequestModel);
            }
            else
            {
                if (!_tokenInitialized)
                {
                    _cachedToken = await GetTokenFromRequestAsync(apiRequestModel);
                    _tokenInitialized = true;
                }
                token = _cachedToken;
            }



            var currentAuth = _httpClient.DefaultRequestHeaders.Authorization;
            if (currentAuth == null || currentAuth.Parameter != token)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<string> GetTokenFromRequestAsync(ApiRequestModel apiRequestModel)
        {
            // Nếu đã có token trong request model
            if (!string.IsNullOrEmpty(apiRequestModel.Token))
                return apiRequestModel.Token;

            // Lấy từ user service
            string token = await _userService.GetTokenCurrentUserAsync();

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Không lấy được token từ phiên làm việc.");

            return token;
        }

        private async Task<ResultAPI<T>> HandleResponse<T>(HttpResponseMessage response, string endpoint)
        {
            var resultAPI = new ResultAPI<T>(StatusCode.Forbidden);

            try
            {
                if (response.Content.Headers.ContentLength.HasValue)
                {
                    var contentLength = response.Content.Headers.ContentLength.Value;

                    if (contentLength > MaxResponseSizeBytes)
                    {
                        _logger.LogError(
                            "Response too large: {Endpoint} | Size: {Size}MB",
                            endpoint, contentLength / 1024.0 / 1024.0);

                        resultAPI.Message = "Dữ liệu phản hồi quá lớn.";
                        resultAPI.Status = StatusCode.InternalServerError;
                        return resultAPI;
                    }

                    if (contentLength > LohThreshold)
                    {
                        _logger.LogWarning(
                            "Large response: {Endpoint} | Size: {Size}KB",
                            endpoint, contentLength / 1024.0);
                    }
                }

                string content;
                if (response.Content.Headers.ContentLength > LohThreshold)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(stream);
                    content = await reader.ReadToEndAsync();
                }
                else
                {
                    content = await response.Content.ReadAsStringAsync();
                }

                if (response.IsSuccessStatusCode)
                {
                    resultAPI.Message = "Thao tác thành công.";
                    resultAPI.Status = StatusCode.OK;

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        resultAPI.Data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    resultAPI.Message = "Hết phiên đăng nhập.";
                    resultAPI.Status = StatusCode.Forbidden;

                    _cachedToken = null;
                    _tokenInitialized = false;
                }
                else
                {
                    var error = JsonSerializer.Deserialize<ResponseErrorAPI>(content, _jsonOptions);
                    var detailError = error?.Errors?.Id != null
                        ? string.Join(", ", error.Errors.Id)
                        : "Lỗi không xác định.";

                    resultAPI.Message = $"Lỗi: {detailError}";
                    resultAPI.Status = StatusCode.InternalServerError;
                    resultAPI.Error = error;
                }
            }
            catch (Exception ex)
            {
                resultAPI.Message = $"Đã có lỗi xảy ra: {ex.Message}";
                resultAPI.Status = StatusCode.InternalServerError;
                _logger.LogError(ex, "HandleResponse<T> error for {Endpoint}", endpoint);
            }

            return resultAPI;
        }

        private async Task<ResultAPI> HandleResponse(HttpResponseMessage response, string endpoint)
        {
            var resultAPI = new ResultAPI(StatusCode.Forbidden);

            try
            {
                if (response.Content.Headers.ContentLength > MaxResponseSizeBytes)
                {
                    resultAPI.Message = "Dữ liệu phản hồi quá lớn.";
                    resultAPI.Status = StatusCode.InternalServerError;
                    return resultAPI;
                }

                string content;
                if (response.Content.Headers.ContentLength > LohThreshold)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(stream);
                    content = await reader.ReadToEndAsync();
                }
                else
                {
                    content = await response.Content.ReadAsStringAsync();
                }

                if (response.IsSuccessStatusCode)
                {
                    resultAPI.Message = "Thao tác thành công.";
                    resultAPI.Status = StatusCode.OK;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    resultAPI.Message = "Hết phiên đăng nhập.";
                    resultAPI.Status = StatusCode.Forbidden;
                    _cachedToken = null;
                    _tokenInitialized = false;
                }
                else
                {
                    var error = JsonSerializer.Deserialize<ResponseErrorAPI>(content, _jsonOptions);
                    var detailError =
                 error?.Errors?.Id != null && error.Errors.Id.Any()
                     ? string.Join(", ", error.Errors.Id)
                     : !string.IsNullOrWhiteSpace(error?.Message)
                        ? error.Message
                        : "Lỗi không xác định.";

                    resultAPI.Message = $"Lỗi: {detailError}";
                    resultAPI.Status = StatusCode.InternalServerError;
                    resultAPI.Error = error;
                }
            }
            catch (Exception ex)
            {
                resultAPI.Message = $"Đã có lỗi xảy ra: {ex.Message}";
                resultAPI.Status = StatusCode.InternalServerError;
                _logger.LogError(ex, "HandleResponse error for {Endpoint}", endpoint);
            }

            return resultAPI;
        }

        private async Task<ResultAPI<byte[]>> HandleFileResponse(HttpResponseMessage response, string endpoint)
        {
            var resultAPI = new ResultAPI<byte[]>(StatusCode.Forbidden);

            try
            {
                if (response.IsSuccessStatusCode)
                {
                    if (response.Content.Headers.ContentLength.HasValue)
                    {
                        var fileSize = response.Content.Headers.ContentLength.Value;

                        if (fileSize > MaxResponseSizeBytes)
                        {
                            _logger.LogError(
                                "File too large: {Endpoint} | Size: {Size}MB",
                                endpoint, fileSize / 1024.0 / 1024.0);

                            resultAPI.Message = "File quá lớn, vượt quá giới hạn cho phép.";
                            resultAPI.Status = StatusCode.BadRequest;
                            return resultAPI;
                        }

                        if (fileSize > LohThreshold)
                        {
                            _logger.LogInformation(
                                "Large file download: {Endpoint} | Size: {Size}KB",
                                endpoint, fileSize / 1024.0);

                            byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent((int)fileSize);
                            try
                            {
                                using var stream = await response.Content.ReadAsStreamAsync();
                                int totalRead = 0;
                                int bytesRead;

                                while ((bytesRead = await stream.ReadAsync(
                                    rentedBuffer.AsMemory(totalRead, (int)fileSize - totalRead))) > 0)
                                {
                                    totalRead += bytesRead;
                                }

                                resultAPI.Data = new byte[totalRead];
                                Array.Copy(rentedBuffer, resultAPI.Data, totalRead);
                            }
                            finally
                            {
                                ArrayPool<byte>.Shared.Return(rentedBuffer);
                            }
                        }
                        else
                        {
                            resultAPI.Data = await response.Content.ReadAsByteArrayAsync();
                        }
                    }
                    else
                    {
                        using var memoryStream = new MemoryStream();
                        await response.Content.CopyToAsync(memoryStream);
                        resultAPI.Data = memoryStream.ToArray();
                    }

                    resultAPI.Message = "Thao tác thành công.";
                    resultAPI.Status = StatusCode.OK;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    resultAPI.Message = "Hết phiên đăng nhập.";
                    resultAPI.Status = StatusCode.Forbidden;
                    _cachedToken = null;
                    _tokenInitialized = false;
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var error = JsonSerializer.Deserialize<ResponseErrorAPI>(content, _jsonOptions);
                        var detailError = error?.Errors?.Id != null
                            ? string.Join(", ", error.Errors.Id)
                            : content;

                        resultAPI.Message = detailError;
                        resultAPI.Error = error;
                    }
                    catch
                    {
                        resultAPI.Message = content;
                    }
                    resultAPI.Status = StatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                resultAPI.Message = $"Đã có lỗi xảy ra: {ex.Message}";
                resultAPI.Status = StatusCode.InternalServerError;
                _logger.LogError(ex, "HandleFileResponse error for {Endpoint}", endpoint);
            }

            return resultAPI;
        }
    }
}

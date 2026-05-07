using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Elearning.Shared.Commons.Services
{
    public class CallServiceRegistryAPI : ICallServiceRegistry
    {

        private readonly TimeSpan _threshold = TimeSpan.FromSeconds(0.5);
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<CallServiceRegistryAPI> _Logger;

        private string? _cachedToken;
        private bool _tokenInitialized = false;

        public CallServiceRegistryAPI(HttpClient httpClient, IConfiguration configuration,
            IRequestContext requestContext, ILogger<CallServiceRegistryAPI> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _requestContext = requestContext;
            _Logger = logger;
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

        #region Private helper
        private void EnsureAuthorization(ApiRequestModel apiRequestModel)
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
                token = GetTokenFromRequest(apiRequestModel);
            }
            else
            {
                if (!_tokenInitialized)
                {
                    _cachedToken = GetTokenFromRequest(apiRequestModel);
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

        private string GetTokenFromRequest(ApiRequestModel apiRequestModel)
        {
            if (!string.IsNullOrEmpty(apiRequestModel.Token))
                return apiRequestModel.Token;

            var currentUser = _requestContext.CurrentUser;

            if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.Token))
                throw new UnauthorizedAccessException("Không lấy được token từ người dùng đang thao tác.");

            return currentUser.Token;
        }

        private string GetFullEndPoint(ApiRequestModel apiRequestModel)
        {
            if (_configuration == null)
                throw new InvalidOperationException("Configuration is not provided.");

            string serviceBaseUrl = apiRequestModel.ApiService == ServicesRegistryEnum.CustomApi
                ? apiRequestModel.ApiServiceCustom ?? throw new KeyNotFoundException("ApiServiceCustom invalid.")
                : _configuration[$"ServicesRegistry:{apiRequestModel.ApiService}"]
                  ?? throw new KeyNotFoundException($"ServicesRegistry {apiRequestModel.ApiService} not found.");

            var fullPath = $"{serviceBaseUrl}/api/v{apiRequestModel.Version}{apiRequestModel.Endpoint}";

            if (apiRequestModel.QueryParams?.Any() == true)
            {
                var queryString = string.Join("&",
                    apiRequestModel.QueryParams.Select(q => $"{q.Key}={Uri.EscapeDataString(q.Value)}"));
                fullPath = $"{fullPath}?{queryString}";
            }

            return fullPath;
        }

        private async Task<ResultAPI<T>> HandleResponse<T>(HttpResponseMessage response, string endpoint)
        {
            var resultAPI = new ResultAPI<T>(StatusCode.Forbidden);
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    resultAPI.Message = "Thao tác thành công.";
                    resultAPI.Status = StatusCode.OK;
                    resultAPI.Data = string.IsNullOrWhiteSpace(content)
                        ? default
                        : JsonSerializer.Deserialize<T>(content, options);
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
                    var error = JsonSerializer.Deserialize<ResponseErrorAPI>(content, options);
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
                Log.Error($"CALL [{endpoint}] FAIL", ex);
            }

            return resultAPI;
        }

        private async Task<ResultAPI> HandleResponse(HttpResponseMessage response, string endpoint)
        {
            var resultAPI = new ResultAPI(StatusCode.Forbidden);
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

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
                    var error = JsonSerializer.Deserialize<ResponseErrorAPI>(content, options);
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
                resultAPI.Message = $"Đá có lỗi xảy ra: {ex.Message}";
                resultAPI.Status = StatusCode.InternalServerError;
                Log.Error($"CALL [{endpoint}] FAIL", ex);
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
                    resultAPI.Data = await response.Content.ReadAsByteArrayAsync();
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
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var error = JsonSerializer.Deserialize<ResponseErrorAPI>(content, options);
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
                Log.Error($"CALL [{endpoint}] FAIL", ex);
            }

            return resultAPI;
        }
        private async Task<T> ExecuteWithLogging<T>(
            string httpMethod,
            ApiRequestModel apiRequestModel,
            Func<string, Task<T>> action)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];

            EnsureAuthorization(apiRequestModel);
            var endpoint = GetFullEndPoint(apiRequestModel);

            try
            {
                var result = await action(endpoint);
                stopwatch.Stop();

                // Log theo threshold để catch slow requests
                if (stopwatch.Elapsed >= _threshold)
                {
                    _Logger.LogWarning(
                        "[{RequestId}] [{Method}] SLOW: {Endpoint} | {Duration}ms | Service: {Service}",
                        requestId, httpMethod, endpoint, stopwatch.ElapsedMilliseconds, apiRequestModel.ApiService);
                }
                else
                {
                    _Logger.LogInformation(
                        "[{RequestId}] [{Method}] OK: {Endpoint} | {Duration}ms",
                        requestId, httpMethod, endpoint, stopwatch.ElapsedMilliseconds);
                }

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _Logger.LogError(ex,
                    "[{RequestId}] [{Method}] ERROR: {Endpoint} | {Duration}ms | {Error}",
                    requestId, httpMethod, endpoint, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }
        #endregion
    }
}

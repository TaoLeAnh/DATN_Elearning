using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Elearning.Shared.Commons.Model.ServiceCustomHttpClient
{
    public class ResultAPI
    {
        public ResultAPI(StatusCode status)
        {
            Status = status;
        }
        public StatusCode Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public ResponseErrorAPI? Error { get; set; }
    }




    /// <summary>
    /// Wrapper kết quả API với kiểu dữ liệu mạnh.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về trong Data</typeparam>
    public class ResultAPI<T>
    {
        /// <summary>
        /// Mã trạng thái (business status)
        /// </summary>
        [JsonPropertyName("status")]
        public StatusCode Status { get; set; }

        /// <summary>
        /// Thông điệp (message)
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Dữ liệu trả về, mạnh kiểu T
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        public ResultAPI(StatusCode status)
        {
            Status = status;
        }

        public ResultAPI(StatusCode status, T data)
            : this(status)
        {
            Data = data;
        }

        public ResultAPI(StatusCode status, string message)
            : this(status)
        {
            Message = message;
        }

        public ResultAPI(StatusCode status, T data, string message)
            : this(status, data)
        {
            Message = message;
        }
        public ResponseErrorAPI? Error { get; set; }
        /// <summary>
        /// Gán dữ liệu và trả về chính nó
        /// </summary>
        public ResultAPI<T> WithData(T data)
        {
            Data = data;
            return this;
        }

        /// <summary>
        /// Gán message và trả về chính nó
        /// </summary>
        public ResultAPI<T> WithMessage(string message)
        {
            Message = message;
            return this;
        }
        /// <summary>
        /// Gán thông tin lỗi và trả về chính nó
        /// </summary>
        public ResultAPI<T> WithError(ResponseErrorAPI error)
        {
            Error = error;
            return this;
        }
    }
}

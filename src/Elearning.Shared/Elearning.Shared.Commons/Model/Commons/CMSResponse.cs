using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.Commons
{
    public class SiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? BuildQuery { get; set; }
        public T? Data { get; set; }
        public Error? Errors { get; set; }
        public StatusCode StatusCode { get; set; }

        public SiResponse()
        {
            Success = true;
            Errors = new Error();
        }

        public static SiResponse<T> SuccessResult(T data, string message = "Thao tác thành công!", string buildQuery = "")
        {
            return new SiResponse<T>
            {
                Success = true,
                Message = message,
                BuildQuery = buildQuery,
                Data = data,
                Errors = null,
                StatusCode = StatusCode.OK
            };
        }


        public static SiResponse<T> ErrorResult(string message, StatusCode statusCode = StatusCode.BadRequest)
        {
            return new SiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = new Error
                {
                    Id = new[] { message }
                }
            };
        }

        public void AddError(string error)
        {
            Success = false;
            if (Errors == null)
            {
                Errors = new Error();
            }
            if (Errors.Id == null)
            {
                Errors.Id = new List<string>().ToArray();
            }

            var errorList = Errors.Id.ToList();
            errorList.Add(error);
            Errors.Id = errorList.ToArray();
        }




    }
}

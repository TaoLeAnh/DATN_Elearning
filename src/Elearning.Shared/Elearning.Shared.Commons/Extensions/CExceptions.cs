using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    /// <summary>
    /// Lỗi chung khi dữ liệu đầu vào không hợp lệ.
    /// </summary>
    [Serializable]
    public class InvalidInputException : Exception
    {
        public InvalidInputException() { }

        public InvalidInputException(string message)
            : base(message) { }

        public InvalidInputException(string message, Exception innerException)
            : base(message, innerException) { }

    }

    public class DatabaseException : Exception
    {
        public string UserMessage { get; }
        public string TechnicalMessage { get; }
        public int? ErrorCode { get; }

        public DatabaseException(string technicalMessage, int? errorCode = null)
            : base(technicalMessage)
        {
            TechnicalMessage = technicalMessage;
            ErrorCode = errorCode;
            UserMessage = MapToUserMessage(errorCode, technicalMessage);
        }

        private static string MapToUserMessage(int? errorCode, string technicalMessage)
        {
            return errorCode switch
            {
                2601 or 2627 => "Dữ liệu đã tồn tại trong hệ thống",
                547 => "Không thể thực hiện do có dữ liệu liên quan",
                515 => "Thiếu thông tin bắt buộc",
                -2 => "Hệ thống đang xử lý, vui lòng thử lại",
                _ when technicalMessage.ToLower().Contains("timeout") => "Hệ thống đang bận, vui lòng thử lại",
                _ => "Có lỗi xảy ra, vui lòng liên hệ quản trị viên"
            };
        }
    }
}

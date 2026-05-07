using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.ServiceCustomHttpClient
{
    public enum StatusCode
    {
        // 1xx: Thông tin
        Informational = 100,
        Continue = 100,           // Tiếp tục
        SwitchingProtocols = 101, // Chuyển giao
        Processing = 102,         // Đang xử lý

        // 2xx: Thành công
        OK = 200,                         // Thành công
        Created = 201,                    // Đã tạo
        Accepted = 202,                   // Đã chấp nhận
        NonAuthoritativeInformation = 203, // Thông tin không có quyền
        NoContent = 204,                  // Không có nội dung
        ResetContent = 205,              // Đặt lại nội dung

        // 3xx: Chuyển hướng
        MultipleChoices = 300,           // Nhiều lựa chọn
        MovedPermanently = 301,          // Di chuyển vĩnh viễn
        Found = 302,                     // Tìm thấy
        SeeOther = 303,                  // Xem mục khác
        NotModified = 304,               // Không thay đổi
        UseProxy = 305,                  // Sử dụng proxy
        TemporaryRedirect = 307,         // Chuyển hướng tạm thời
        PermanentRedirect = 308,         // Chuyển hướng vĩnh viễn

        // 4xx: Lỗi của client
        BadRequest = 400,                // Yêu cầu không hợp lệ
        Unauthorized = 401,              // Không được phép
        PaymentRequired = 402,           // Cần thanh toán
        Forbidden = 403,                 // Bị cấm
        NotFound = 404,                  // Không tìm thấy
        MethodNotAllowed = 405,          // Phương thức không chấp nhận
        NotAcceptable = 406,             // Không chấp nhận
        ProxyAuthenticationRequired = 407, // Cần xác thực proxy
        RequestTimeout = 408,            // Yêu cầu hết hạn
        Conflict = 409,                  // Xung đột
        Gone = 410,                      // Đã mất đi
        LengthRequired = 411,            // Cần có độ dài
        PreconditionFailed = 412,        // Điều kiện tiên quyết thất bại
        PayloadTooLarge = 413,           // Thực thể yêu cầu quá lớn
        URITooLong = 414,                // URI quá dài
        UnsupportedMediaType = 415,      // Loại phương tiện không được hỗ trợ
        RangeNotSatisfiable = 416,       // Không thể sắp xếp
        ExpectationFailed = 417,         // Trạng thái thất bại
        ImATeapot = 418,                 // Tài nguyên là... (Trà hay cà phê :) )
        UpgradeRequired = 426,           // Quá thời

        // 5xx: Lỗi của server
        InternalServerError = 500,       // Lỗi máy chủ
        NotImplemented = 501,            // Chưa thực hiện
        BadGateway = 502,                // Cổng không hợp lệ
        ServiceUnavailable = 503,        // Dịch vụ không khả dụng
        GatewayTimeout = 504,            // Cổng hết thời gian
        HTTPVersionNotSupported = 505,   // Phiên bản HTTP không được hỗ trợ

        // Mã lỗi tùy chỉnh
        SystemConfigurationError = 600,  // Lỗi cấu hình hệ thống
        DatabaseConnectionError = 601,   // Lỗi kết nối CSDL
        DataProcessingError = 602,       // Lỗi xử lý dữ liệu
        ApplicationSecurityError = 603,  // Lỗi bảo mật ứng dụng
        ApplicationPerformanceError = 604, // Lỗi hiệu suất ứng dụng
        SystemIntegrationError = 605     // Lỗi tích hợp hệ thống

        // Có thể thêm các mã lỗi tùy chỉnh khác ở đây
    }
}

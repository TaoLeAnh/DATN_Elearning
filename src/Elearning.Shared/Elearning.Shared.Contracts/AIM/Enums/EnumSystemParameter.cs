using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Enums
{
    public enum EnumSystemParameter
    {
        [Description("Không xác định")]
        None = 0,

        [Description("Bật tắt header")]
        HeaderEnabled,

        [Description("Danh sách menu sẽ tự đông add vào menu bàn làm việc. giá trị là list ID cách nhau bởi dấu")]
        BANLAMVIEC,
        [Description("Tham số này dành cho riêng cho mỗi cổng dùng để sử dụng cho tính năng biên tập AI")]
        THAMSOBIENTAPAI,

        [Description("Cấu hình thời gian cache tại bàn làm việc")]
        SetTimeCacheBLV,

        [Description("Số tin nóng hiển thị trang chủ")]
        SoLuongTinNongMoiHienThi,

        #region Analytis
        [Description("Analytics: Domain, Username, Password, WebsiteId, Script")]
        AnalyticsConfig,


        #endregion

        [Description("App Version")]
        AppVersion,

        [Description("App Information")]
        AppInformation,
        [Description("Domain cổng")]
        DomainUIPublising,

        [Description("Thông tin tham số lịch tích hợp")]
        THAMSOLICHTICHHOP,

        [Description("Cấu hình dành cho chuyển đổi giọng nói")]
        SettingTTS,
    }
}

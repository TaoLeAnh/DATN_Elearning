using System.ComponentModel;

namespace Elearning.Shared.Commons.Model.Extentions.Redis
{
    public enum RedisTypeKey
    {
        [Description("Không xác định")]
        None,
        [Description("Key thuộc lõi phần mềm")]
        Core,
        [Description("Key session")]
        Session,
        [Description("Liên quan mọi thứ đến người dùng quyền,vai trò,....")]
        User,
        [Description("Caching ngoài khai thác")]
        Publishing,
        [Description("Liên quan đến hồ sơ trong quản trị")]
        HoSo,
        [Description("Thuộc xử lý báo cáo")]
        BaoCao,
        [Description("Key trung chuyển giữa các service với api external")]
        External,
        [Description("Lưu trữ tham số hệ thống hoạt động xuyên giữa các service")]
        SystemConfiguration

    }
}

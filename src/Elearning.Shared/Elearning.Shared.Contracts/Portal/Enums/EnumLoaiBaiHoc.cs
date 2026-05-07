using System.ComponentModel;

namespace Elearning.Shared.Contracts.Portal.Enums
{
    public enum EnumLoaiBaiHoc
    {
        [Description("Video bài giảng")]
        Video = 1,

        [Description("Văn bản/Bài đọc")]
        Text = 2,

        [Description("Tài liệu (PDF/Word)")]
        Document = 3,

        [Description("Bài kiểm tra (Quiz)")]
        Quiz = 4
    }
}

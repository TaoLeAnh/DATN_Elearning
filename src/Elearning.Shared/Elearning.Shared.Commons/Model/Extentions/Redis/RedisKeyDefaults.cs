using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.Extentions.Redis
{
    public static class RedisKeys
    {
        /// <summary>
        /// Thông tin phiên bản phần mềm hiện tại
        /// </summary>
        public const string AppVersion = "APP_VERSION";

        /// <summary>
        /// Mô tả thông tin phần mềm
        /// </summary>
        public const string AppInformation = "APP_INFORMATION";

        /// <summary>
        /// Chứa tòn bộ trạng thái để từ trạng thái >> tìm ra nút động
        /// </summary>

        public const string WorkFlowStatesWithActions = "WORKFLOW-STATES-WITH-ACTIONS";

        public const string ThamSoHeThong = "THAMSOHETHONG";

        /// <summary>
        /// Key danh mục bài viết phục vụ cho các select trong module tin bài
        /// </summary>
        public const string DataDanhMucTinBai = "DATA-DANHMUC-TINBAI";
        /// <summary>
        /// Key chứa dữ liệu cổng chính lấy danh mục tin bài
        /// </summary>
        public const string TieuDeDanhMucTinBai = "TIEUDE-DANHMUC-TINBAI";
        /// <summary>
        /// Key chứa dữ liệu danh sách bài viết đang chỉnh sửa
        /// </summary>
        public const string BaiVietDangChinhSua = "BAIVIETDANGCHINHSUA";
        /// <summary>
        /// Key chứa Script phân tích
        /// </summary>
        public const string SCRIPTANALYTIC = "SCRIPT_ANALYTIC";

        /// <summary>
        /// Key chứa các thống kê
        /// </summary>
        public const string ThongKe = "THONGKE";

        /// <summary>
        /// Key chứa thống kê truy cập
        /// </summary>
        public const string ThongKeAnalytic = "THONGKE_ANALYTIC";




        /// <summary>
        /// Lưu giá trị hàng đợi người dùng (Client) đang online ngoài khai thác
        /// </summary>
        public const string ThongKeDangOnlineActiveUser = "ONLINE_TOTAL_NUMBER";


        public const string LichLamViecBTP = "LICHLAMVIEC_BTP";





    }
}

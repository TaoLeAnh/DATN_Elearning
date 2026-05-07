using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elearning.Publising.UI.Pages
{
    public class ExamModel : PageModel
    {
        private readonly ICallServiceRegistry _callService;

        // Khai báo các List để chứa dữ liệu đổ ra giao diện
        public List<PublicKyThiDto> DeThiThuList { get; set; } = new();
        public List<PublicKyThiDto> DeChinhThucList { get; set; } = new();
        public List<PublicKyThiDto> DeThiLiveList { get; set; } = new();

        // Nhận tham số Môn học từ URL (ví dụ: ?MonHoc=1)
        [BindProperty(SupportsGet = true)]
        public int? MonHoc { get; set; }

        // Inject ICallServiceRegistry theo đúng chuẩn dự án
        public ExamModel(ICallServiceRegistry callService)
        {
            _callService = callService;
        }

        public async Task OnGetAsync()
        {
            try
            {
                // 1. Build endpoint kèm tham số QueryString nếu có
                string endpoint = "/Publising/KyThi";
                if (MonHoc.HasValue)
                {
                    endpoint += $"?monHoc={MonHoc.Value}";
                }

                // 2. Chuẩn bị Request Model gọi sang API Publishing
                var req = new ApiRequestModel
                {
                    ApiService = ServicesRegistryEnum.ServicePublising,
                    Endpoint = endpoint,
                    HasAuthorization = false // Không cần Token vì đây là trang Public
                };

                // 3. Thực thi gọi API
                var res = await _callService.Get<List<PublicKyThiDto>>(req);

                // 4. Nếu API trả về OK (200), tiến hành chia dữ liệu
                if (res.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && res.Data != null)
                {
                    var allExams = res.Data;

                    // Lọc ra đề thi thử
                    DeThiThuList = allExams.Where(x => x.LoaiDeThi == EnumLoaiDeThi.DeThiThu).ToList();

                    // Lọc ra đề chính thức
                    DeChinhThucList = allExams.Where(x => x.LoaiDeThi == EnumLoaiDeThi.DeChinhThuc).ToList();

                    // THÊM DÒNG NÀY: Lọc ra kỳ thi trực tiếp
                    // (Lưu ý: Bác thay 'ThiLive' bằng đúng tên Enum bác đang dùng trong EnumLoaiDeThi nhé)
                    DeThiLiveList = allExams.Where(x => x.LoaiDeThi == EnumLoaiDeThi.DeThiLive).ToList();
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu hệ thống API mất kết nối
                Console.WriteLine($"Lỗi khi tải danh sách đề thi: {ex.Message}");
            }
        }
        public class RandomRequestForm
        {
            public int MonHocId { get; set; }
        }

        // BỔ SUNG HÀM NHẬN SỰ KIỆN TỪ NÚT "TẠO ĐỀ NGẪU NHIÊN"
        public async Task<IActionResult> OnPostGenerateRandomExam([FromBody] RandomRequestForm form)
        {
            try
            {
                var req = new ApiRequestModel
                {
                    ApiService = ServicesRegistryEnum.ServicePublising,
                    Endpoint = "/Publising/KyThi/random",
                    HasAuthorization = false // Không cần Token
                };

                // Gọi API backend với data là MonHocId
                var res = await _callService.Post<Guid>(req, new { MonHocId = form.MonHocId });

                if (res.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && res.Data != Guid.Empty)
                {
                    // Trả về JSON chứa ID đề thi mới để JS chuyển trang
                    return new JsonResult(new { success = true, newExamId = res.Data });
                }

                return new JsonResult(new { success = false, message = res.Message ?? "Ngân hàng câu hỏi không đủ để tạo đề." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra khi tạo đề: " + ex.Message });
            }
        }
    }
}
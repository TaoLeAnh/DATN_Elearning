using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Elearning.Publising.UI.Pages
{
    public class MyCoursesModel : PageModel
    {
        private readonly ICallServiceRegistry _callService;

        public MyCoursesModel(ICallServiceRegistry callService)
        {
            _callService = callService;
        }

        public List<MyCourseDto> LstMyCourses { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Giả sử bác đã có middleware bọc việc lấy UserId
            // Nếu dùng Identity, lấy từ User.FindFirstValue(ClaimTypes.NameIdentifier)
            var myToken = User.FindFirst("AccessToken")?.Value;
            ApiRequestModel apiRequest = new ApiRequestModel()
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = "/Publising/DangKyKhoaHoc/my-courses",
                HasAuthorization = true, // Vì trang này bắt buộc đăng nhập
                Token = myToken
            };

            var result = await _callService.Get<List<MyCourseDto>>(apiRequest);
            if (result.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && result.Data != null)
            {
                LstMyCourses = result.Data;
            }
        }
    }
}

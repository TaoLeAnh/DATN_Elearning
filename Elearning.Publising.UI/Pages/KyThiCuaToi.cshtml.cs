using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Elearning.Publising.UI.Pages
{
    public class KyThiCuaToiModel : PageModel
    {
        private readonly ICallServiceRegistry _callService;

        public KyThiCuaToiModel(ICallServiceRegistry callService)
        {
            _callService = callService;
        }

        public List<BaiLamDto> LstMyExams { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            var myToken = User.FindFirst("AccessToken")?.Value;
            ApiRequestModel apiRequest = new ApiRequestModel()
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = $"/Publising/KyThi/my-exams?userId={userId}",
                HasAuthorization = true,
                Token = myToken
            };

            var result = await _callService.Get<List<BaiLamDto>>(apiRequest);
            if (result.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && result.Data != null)
            {
                LstMyExams = result.Data;
            }
        }
    }
}
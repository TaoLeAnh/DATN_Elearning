using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Elearning.Publising.UI.Pages
{
    public class ReviewBaiThiModel : PageModel
    {
        private readonly ICallServiceRegistry _callService;

        public ReviewBaiThiModel(ICallServiceRegistry callService)
        {
            _callService = callService;
        }

        public BaiLamReviewDto ReviewData { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid baiLamId)
        {
            if (baiLamId == Guid.Empty) return RedirectToPage("/KyThiCuaToi");

            var myToken = User.FindFirst("AccessToken")?.Value;

            var apiRequest = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = $"/Publising/KyThi/bai-lam/{baiLamId}/review",
                HasAuthorization = true,
                Token = myToken
            };

            var result = await _callService.Get<BaiLamReviewDto>(apiRequest);

            if (result.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && result.Data != null)
            {
                ReviewData = result.Data;
                return Page();
            }

            ReviewData = null;
            return Page();
        }
    }
}
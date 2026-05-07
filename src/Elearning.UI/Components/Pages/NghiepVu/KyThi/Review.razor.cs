using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Elearning.UI.Components.Pages.NghiepVu.KyThi
{
    public partial class Review : ComponentBase
    {
        [Parameter] public Guid BaiLamId { get; set; }

        [Inject] private ICallServiceRegistry CallService { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager NavManager { get; set; } = default!;

        private BaiLamReviewDto? Model;
        private bool IsLoading = true;

        protected override async Task OnInitializedAsync()
        {
            if (BaiLamId != Guid.Empty)
            {
                var req = new ApiRequestModel
                {
                    ApiService = ServicesRegistryEnum.ServicePortal,
                    Endpoint = $"/BaiLam/{BaiLamId}/review",
                    HasAuthorization = true
                };

                var res = await CallService.Get<BaiLamReviewDto>(req);

                if (res.Status == StatusCode.OK && res.Data != null)
                {
                    Model = res.Data;
                }
                else
                {
                    ToastService.ShowError(res.Message ?? "Không thể tải chi tiết bài làm.");
                }
            }
            IsLoading = false;
        }

        protected void GoBack()
        {
            if (Model != null && Model.BaiLamId != Guid.Empty)
            {
                // Quay lại trang danh sách bài làm của kỳ thi đó
                NavManager.NavigateTo($"/nghiep-vu/quan-ly-ky-thi/{Model.BaiLamId}/bai-lam");
            }
            else
            {
                NavManager.NavigateTo("/nghiep-vu/quan-ly-ky-thi");
            }
        }
    }
}

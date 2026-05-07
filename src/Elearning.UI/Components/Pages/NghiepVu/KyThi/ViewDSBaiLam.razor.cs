using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Commons.Querys.Grid;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Elearning.UI.Components.Pages.NghiepVu.KyThi
{
    public partial class ViewDSBaiLam : ComponentBase
    {
        [Parameter] public Guid KyThiId { get; set; }

        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private ICallServiceRegistry CallService { get; set; } = default!;
        [Inject] private NavigationManager NavManager { get; set; } = default!;

        protected string? SearchKeyword { get; set; } = string.Empty;

        // Setup sort theo Mã SV và Tên SV
        private ColumnKeyGridSort<BaiLamDto> _roleMaSVSort = new("MaSinhVien");
        private ColumnKeyGridSort<BaiLamDto> _roleTenSVSort = new("TenSinhVien");

        protected FluentDataGrid<BaiLamDto> Grid { get; set; } = default!;
        protected PaginationState pagination = new PaginationState { ItemsPerPage = 10 };

        private async ValueTask<GridItemsProviderResult<BaiLamDto>> LoadDatas(GridItemsProviderRequest<BaiLamDto> request)
        {
            var apiRequest = new ApiRequestModel { ApiService = ServicesRegistryEnum.ServicePortal, Endpoint = "/BaiLam/getpaged-admin", HasAuthorization = true };

            // Ép sang BaiLamQuery để truyền được KyThiId
            var query = new BaiLamQuery
            {
                KyThiId = this.KyThiId, // Bắt buộc truyền ID Kỳ thi để lọc đúng danh sách
                draw = 1,
                Keyword = SearchKeyword,
                sort = request.GetSortByProperties().Select(s => new Sort { field = s.PropertyName, dir = s.Direction == SortDirection.Ascending ? "asc" : "desc" }).ToList(),
                gridRequest = new GridRequest { page = request.StartIndex / pagination.ItemsPerPage + 1, pageSize = pagination.ItemsPerPage }
            };

            var result = await CallService.Post<DataTableJson<BaiLamDto>>(apiRequest, query);

            if (result.Status == StatusCode.OK && result.Data != null)
            {
                var data = result.Data.Data.Select((item, idx) => { item.STT = request.StartIndex + idx + 1; return item; }).ToList();
                return GridItemsProviderResult.From(data, result.Data.RecordsTotal);
            }
            return GridItemsProviderResult.From(new List<BaiLamDto>(), 0);
        }

        protected async Task HandleSearchChanged() => await Grid.RefreshDataAsync();

        private async Task RefreshData(int value)
        {
            pagination.ItemsPerPage = value;
            await pagination.SetCurrentPageIndexAsync(0);
        }

        // Điều hướng sang trang xem chi tiết (Review) tờ giấy thi của sinh viên
        protected void ViewReview(Guid baiLamId)
        {
            NavManager.NavigateTo($"/nghiep-vu/quan-ly-ky-thi/bai-lam/{baiLamId}/review");
        }

        protected void GoBack()
        {
            NavManager.NavigateTo("/nghiep-vu/quan-ly-ky-thi");
        }
    }
}

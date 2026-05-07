using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Commons.Querys.Grid;
using Elearning.Shared.Commons.Querys.ModalQuery;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.UI.Application.Dtos;
using Elearning.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Elearning.UI.Components.Pages.NghiepVu.BoCauHoiOnTap
{
    public partial class Index : ComponentBase
    {
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private ICallServiceRegistry CallService { get; set; } = default!;
        [Inject] private NavigationManager NavManager { get; set; } = default!;

        protected string? SearchKeyword { get; set; } = string.Empty;
        protected FluentDataGrid<BoCauHoiOnTapDto> Grid { get; set; } = default!;
        protected PaginationState pagination = new PaginationState { ItemsPerPage = 10 };

        protected void GoToAddPage() => NavManager.NavigateTo("/nghiep-vu/quan-ly-bo-cau-hoi/edit");
        protected void GoToEditPage(Guid id) => NavManager.NavigateTo($"/nghiep-vu/quan-ly-bo-cau-hoi/edit/{id}");

        protected async Task OpenDetailsModal(Guid Id)
        {
            var parameters = new EditOrUpdateParametersDto { Id = Id };
            await DialogService.ShowDialogAsync<View>(parameters, new DialogParameters { Title = "Chi tiết Bộ câu hỏi", Width = "800px", Modal = true });
        }

        protected async Task OpenModalDelete(Guid id)
        {
            var dialog = await DialogService.ShowDialogAsync<ModalConfirm>(new DialogParameters());
            var resultDialog = await dialog.Result;
            if (!resultDialog.Cancelled && resultDialog.Data is bool success && success)
            {
                var apiRequest = new ApiRequestModel { ApiService = ServicesRegistryEnum.ServicePortal, Endpoint = $"/BoCauHoiOnTap/{id}", HasAuthorization = true };
                var result = await CallService.Delete(apiRequest);
                if (result.Status == StatusCode.OK) { await RefreshGrid(); ToastService.ShowSuccess("Xóa thành công!"); }
                else ToastService.ShowError(result.Message ?? "Lỗi khi xóa.");
            }
        }

        private async ValueTask<GridItemsProviderResult<BoCauHoiOnTapDto>> LoadDatas(GridItemsProviderRequest<BoCauHoiOnTapDto> request)
        {
            var apiRequest = new ApiRequestModel { ApiService = ServicesRegistryEnum.ServicePortal, Endpoint = "/BoCauHoiOnTap/getpaged", HasAuthorization = true };
            var baseQuery = new BaseQuery
            {
                draw = 1,
                SearchIn = new List<string> { "TenBo", "MoTa" },
                Keyword = SearchKeyword,
                sort = request.GetSortByProperties().Select(s => new Sort { field = s.PropertyName, dir = s.Direction == SortDirection.Ascending ? "asc" : "desc" }).ToList(),
                gridRequest = new GridRequest { page = request.StartIndex / pagination.ItemsPerPage + 1, pageSize = pagination.ItemsPerPage }
            };

            var result = await CallService.Post<DataTableJson<BoCauHoiOnTapDto>>(apiRequest, baseQuery);
            if (result.Status == StatusCode.OK && result.Data != null)
            {
                var data = result.Data.Data.Select((item, idx) => { item.STT = request.StartIndex + idx + 1; return item; }).ToList();
                return GridItemsProviderResult.From(data, result.Data.RecordsTotal);
            }
            return GridItemsProviderResult.From(new List<BoCauHoiOnTapDto>(), 0);
        }

        protected async Task HandleSearchChanged() => await Grid.RefreshDataAsync();
        private async Task RefreshData(int value) { pagination.ItemsPerPage = value; await pagination.SetCurrentPageIndexAsync(0); }
        protected async Task RefreshGrid() => await Grid.RefreshDataAsync();
    }
}

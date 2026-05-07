using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Commons.Querys.Grid;
using Elearning.Shared.Commons.Querys.ModalQuery;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.UI.Application.Dtos;
using Elearning.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Elearning.UI.Components.Pages.NghiepVu.ChuongHoc
{
    public partial class Index : ComponentBase
    {
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private ICallServiceRegistry CallService { get; set; } = default!;

        protected string? SearchKeyword { get; set; } = string.Empty;
        private ColumnKeyGridSort<ChuongHocDto> _roleTenChuongSort = new("TenChuong");
        private ColumnKeyGridSort<ChuongHocDto> _roleThuTuSort = new("ThuTu");

        protected FluentDataGrid<ChuongHocDto> Grid { get; set; } = default!;
        protected PaginationState pagination = new PaginationState { ItemsPerPage = 10 };
        private bool IsLoadingSync { get; set; } = false;

        protected async Task OpenAddModal()
        {
            var parameters = new EditOrUpdateParametersDto { Id = Guid.Empty, IsEditMode = false, OnRefresh = EventCallback.Factory.Create(this, RefreshGrid) };
            await DialogService.ShowDialogAsync<Edit>(parameters, new DialogParameters 
            { 
                Title = "Thêm mới chương học", 
                Width = "600px", 
                Modal = true 
            });
        }

        protected async Task EditAction(Guid Id)
        {
            var parameters = new EditOrUpdateParametersDto 
            { 
                Id = Id, 
                IsEditMode = true, 
                OnRefresh = EventCallback.Factory.Create(this, RefreshGrid) 
            };
            await DialogService.ShowDialogAsync<Edit>(parameters, new DialogParameters
            { 
                Title = "Chỉnh sửa chương học", 
                Width = "600px", 
                Modal = true 
            });
        }

        protected async Task OpenDetailsModal(Guid Id)
        {
            var parameters = new EditOrUpdateParametersDto { Id = Id };
            await DialogService.ShowDialogAsync<View>(parameters, new DialogParameters 
            { 
                Title = "Chi tiết chương học", 
                Width = "600px", 
                Modal = true, 
                PrimaryAction = null, 
                SecondaryAction = null 
            });
        }

        protected async Task OpenModalDelete(Guid id)
        {
            var dialog = await DialogService.ShowDialogAsync<ModalConfirm>(new DialogParameters());
            var resultDialog = await dialog.Result;
            if (!resultDialog.Cancelled && resultDialog.Data is bool success && success)
            {
                var apiRequest = new ApiRequestModel 
                { 
                    ApiService = ServicesRegistryEnum.ServicePortal, 
                    Endpoint = $"/ChuongHoc/{id}" ,
                    HasAuthorization = true
                };
                var result = await CallService.Delete(apiRequest);
                if (result.Status == StatusCode.OK)
                {
                    await RefreshGrid();
                    ToastService.ShowSuccess("Xóa thành công!");
                }
                else ToastService.ShowError(result.Message ?? "Lỗi khi xóa.");
            }
        }

        private async ValueTask<GridItemsProviderResult<ChuongHocDto>> LoadDatas(GridItemsProviderRequest<ChuongHocDto> request)
        {
            var apiRequest = new ApiRequestModel 
            { 
                ApiService = ServicesRegistryEnum.ServicePortal, 
                Endpoint = "/ChuongHoc/GetPaged" ,
                HasAuthorization = true
            };
            var baseQuery = new BaseQuery
            {
                draw = 1,
                SearchIn = new List<string> { "TenChuong" },
                Keyword = SearchKeyword,
                sort = request.GetSortByProperties().Select(s => new Sort { field = s.PropertyName, dir = s.Direction == SortDirection.Ascending ? "asc" : "desc" }).ToList(),
                gridRequest = new GridRequest { page = request.StartIndex / pagination.ItemsPerPage + 1, pageSize = pagination.ItemsPerPage }
            };

            var result = await CallService.Post<DataTableJson<ChuongHocDto>>(apiRequest, baseQuery);
            if (result.Status == StatusCode.OK && result.Data != null)
            {
                var data = result.Data.Data.Select((item, idx) => { item.STT = request.StartIndex + idx + 1; return item; }).ToList();
                return GridItemsProviderResult.From(data, result.Data.RecordsTotal);
            }
            return GridItemsProviderResult.From(new List<ChuongHocDto>(), 0);
        }

        protected async Task HandleSearchChanged() => await Grid.RefreshDataAsync();
        private async Task RefreshData(int value) { pagination.ItemsPerPage = value; await pagination.SetCurrentPageIndexAsync(0); }
        protected async Task RefreshGrid() => await Grid.RefreshDataAsync();
    }
}

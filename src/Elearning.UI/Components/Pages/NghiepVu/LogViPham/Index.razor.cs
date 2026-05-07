using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Commons.Querys.Grid;
using Elearning.Shared.Commons.Querys.ModalQuery;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.UI.Application.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Elearning.UI.Components.Pages.NghiepVu.LogViPham
{
    public partial class Index : ComponentBase
    {
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private ICallServiceRegistry CallService { get; set; } = default!;

        protected string? SearchKeyword { get; set; } = string.Empty;

        // Khai báo các cột có thể Sort
        private ColumnKeyGridSort<LogViPhamDto> _sortNguoiDung = new("TenNguoiDung");
        private ColumnKeyGridSort<LogViPhamDto> _sortThoiDiem = new("ThoiDiemViPham");
        private ColumnKeyGridSort<LogViPhamDto> _sortLoaiViPham = new("LoaiViPham");

        protected FluentDataGrid<LogViPhamDto> Grid { get; set; } = default!;
        protected PaginationState pagination = new PaginationState { ItemsPerPage = 10 };

        // Mở popup Xem chi tiết
        protected async Task OpenDetailsModal(Guid Id)
        {
            var parameters = new EditOrUpdateParametersDto { Id = Id };
            await DialogService.ShowDialogAsync<View>(parameters, new DialogParameters
            {
                Title = "Chi tiết biên bản vi phạm",
                Width = "600px",
                Modal = true
            });
        }

        // Lấy dữ liệu từ Backend
        private async ValueTask<GridItemsProviderResult<LogViPhamDto>> LoadDatas(GridItemsProviderRequest<LogViPhamDto> request)
        {
            var apiRequest = new ApiRequestModel { ApiService = ServicesRegistryEnum.ServicePortal, Endpoint = "/LogViPham/getpaged", HasAuthorization = true };

            var baseQuery = new BaseQuery
            {
                draw = 1,
                SearchIn = new List<string> { "ChiTiet" },
                Keyword = SearchKeyword,
                sort = request.GetSortByProperties().Select(s => new Sort { field = s.PropertyName, dir = s.Direction == SortDirection.Ascending ? "asc" : "desc" }).ToList(),
                gridRequest = new GridRequest { page = request.StartIndex / pagination.ItemsPerPage + 1, pageSize = pagination.ItemsPerPage }
            };

            // Nếu người dùng chưa bấm sort cột nào, mặc định sort theo thời gian vi phạm mới nhất
            if (!baseQuery.sort.Any())
            {
                baseQuery.sort.Add(new Sort { field = "ThoiDiemViPham", dir = "desc" });
            }

            var result = await CallService.Post<DataTableJson<LogViPhamDto>>(apiRequest, baseQuery);
            if (result.Status == StatusCode.OK && result.Data != null)
            {
                var data = result.Data.Data.Select((item, idx) => { item.STT = request.StartIndex + idx + 1; return item; }).ToList();
                return GridItemsProviderResult.From(data, result.Data.RecordsTotal);
            }
            return GridItemsProviderResult.From(new List<LogViPhamDto>(), 0);
        }

        protected async Task HandleSearchChanged() => await Grid.RefreshDataAsync();
        private async Task RefreshData(int value) { pagination.ItemsPerPage = value; await pagination.SetCurrentPageIndexAsync(0); }
        protected async Task RefreshGrid() => await Grid.RefreshDataAsync();
    }
}

using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Commons.Querys.Grid;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Elearning.Publising.UI.Pages
{
    public class DanhSachKhoaHocModel : PageModel
    {
        private readonly ICallServiceRegistry _callService;
        public DanhSachKhoaHocModel(ICallServiceRegistry callService) { _callService = callService; }

        public List<KhoaHocDto> LstKhoaHoc { get; set; } = new();
        public List<MonHocItem> LstMonHoc { get; set; } = new();

        // Các thuộc tính phục vụ phân trang
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int? SelectedMonHoc { get; set; }

        public async Task OnGetAsync(int? monHoc, int p = 1)
        {
            CurrentPage = p;
            SelectedMonHoc = monHoc;

            // 1. Lấy danh sách môn học từ Enum (giữ nguyên logic cũ của bác)
            LstMonHoc = Enum.GetValues(typeof(MonHocEnum))
    .Cast<MonHocEnum>()
    .Select(e => {
        var field = e.GetType().GetField(e.ToString());

        var description = field?.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description
                       ?? field?.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.Name
                       ?? e.ToString();

        return new MonHocItem
        {
            Value = (int)e,
            Text = description, // Gán thẳng cái mô tả vào thuộc tính Text
            Icon = GetIconByMonHoc(e)
        };
    }).ToList();


            // 2. Chuẩn bị Query gọi API GetPaged
            var query = new KhoaHocQuery
            {
                MonHoc = monHoc.HasValue ? (MonHocEnum)monHoc.Value : null,
                gridRequest = new GridRequest
                {
                    page = p,
                    pageSize = 12 // Số khóa học mỗi trang
                }
            };

            var apiRequest = new ApiRequestModel
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = "/Publising/KhoaHoc/getpaged",
                HasAuthorization = false
            };

            var result = await _callService.Post<DataTableJson>(apiRequest, query);

            if (result.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && result.Data != null)
            {
                // Bác thay 2 dòng cũ bằng đoạn này:
                if (result.Data.data is System.Text.Json.JsonElement jsonElement)
                {
                    // Cách này an toàn và chuẩn nhất khi dùng System.Text.Json
                    var rawJson = jsonElement.GetRawText();
                    LstKhoaHoc = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KhoaHocDto>>(rawJson) ?? new();
                }
                else
                {
                    // Nếu nó đã là Object rồi thì cứ ép kiểu (Cast) thôi
                    LstKhoaHoc = (List<KhoaHocDto>)result.Data.data;
                }

                // Tính tổng số trang (giữ nguyên)
                TotalPages = (int)Math.Ceiling((double)result.Data.recordsTotal / 12);
            }
        }

        private string GetIconByMonHoc(MonHocEnum m) => m switch
        {
            MonHocEnum.Toan => "🧮",
            MonHocEnum.NguVan => "📖",
            MonHocEnum.TiengAnh => "✍️",
            _ => "📚"
        };
    }


    public class MonHocItem
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
    }
}

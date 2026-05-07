using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Elearning.Publising.UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICallServiceRegistry _callService;

        public IndexModel(ICallServiceRegistry callService)
        {
            _callService = callService;
        }
        public Dictionary<MonHocEnum, List<KhoaHocDto>> DictKhoaHocTheoMon { get; set; } = new();
        public List<KhoaHocDto> LstKhoaHocTHCS { get; set; } = new();
        public Dictionary<MonHocEnum, List<HoSoGiaoVienDto>> DictGiaoVienTheoMon { get; set; } = new();
        public int TongSoHocVien { get; set; }
        public async Task OnGetAsync()
        {
            var allCoursesTHPT = await FetchKhoaHoc("THPT", 20);

            if (allCoursesTHPT != null && allCoursesTHPT.Any())
            {
                DictKhoaHocTheoMon = allCoursesTHPT
                    .GroupBy(x => x.MonHoc)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            LstKhoaHocTHCS = await FetchKhoaHoc("THCS", 8);

            var allTeachers = await FetchTatCaGiaoVien();
            if (allTeachers != null && allTeachers.Any())
            {
                DictGiaoVienTheoMon = allTeachers
                    .GroupBy(x => x.MonHocChuyenMon)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            try
            {
                var reqCount = new ApiRequestModel
                {
                    ApiService = ServicesRegistryEnum.ServicePublising,
                    // Lưu ý: Bác đảm bảo bên dự án API Publishing đã có viết API này nhé
                    Endpoint = "/Publising/DangKyKhoaHoc/count-all",
                    HasAuthorization = false
                };

                var resCount = await _callService.Get<int>(reqCount);
                if (resCount.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK)
                {
                    TongSoHocVien = resCount.Data;
                }
                else
                {
                    TongSoHocVien = 7883212; // Số giả lập nếu API trả về lỗi
                }
            }
            catch (Exception)
            {
                TongSoHocVien = 7883212; // Số giả lập nếu mất kết nối
            }
        }

        private async Task<List<KhoaHocDto>> FetchKhoaHoc(string capHoc, int limit)
        {
            var query = new
            {
                start = 0,
                length = limit,
                draw = 1,

                PageIndex = 1,
                PageSize = limit
            };

            ApiRequestModel apiRequest = new ApiRequestModel()
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = $"/Publising/KhoaHoc/getpaged"
            };

            var result = await _callService.Post<DataTableJson>(apiRequest, query);

            if (result.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && result.Data != null)
            {
                var jsonStr = JsonSerializer.Serialize(result.Data.data);
                return JsonSerializer.Deserialize<List<KhoaHocDto>>(jsonStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<KhoaHocDto>();
            }
            return new List<KhoaHocDto>();
        }
        private async Task<List<HoSoGiaoVienDto>> FetchTatCaGiaoVien()
        {
            ApiRequestModel apiRequest = new ApiRequestModel()
            {
                ApiService = ServicesRegistryEnum.ServicePublising,
                Endpoint = "/Publising/HoSoGiaoVien" // Endpoint API Publising đã viết
            };

            var result = await _callService.Get<List<HoSoGiaoVienDto>>(apiRequest);

            if (result.Status == Elearning.Shared.Commons.Model.ServiceCustomHttpClient.StatusCode.OK && result.Data != null)
            {
                return result.Data;
            }
            return new List<HoSoGiaoVienDto>();
        }
        public string GetSubjectKey(MonHocEnum monHoc)
        {
            return monHoc.ToString().ToLower();
        }
        public string GetTenMonHoc(MonHocEnum monHoc)
        {
            return monHoc switch
            {
                MonHocEnum.Toan => "Toán học",
                MonHocEnum.NguVan => "Ngữ văn",
                MonHocEnum.TiengAnh => "Tiếng Anh",
                MonHocEnum.VatLy => "Vật lí",
                MonHocEnum.HoaHoc => "Hóa học",
                MonHocEnum.SinhHoc => "Sinh học",
                MonHocEnum.LichSu => "Lịch sử",
                MonHocEnum.DiaLy => "Địa lí",
                MonHocEnum.GDCD => "Giáo dục công dân",
                MonHocEnum.KhoaHocVaDocHieu => "Khoa học & Đọc hiểu",
                _ => "Khác"
            };
        }
    }
}

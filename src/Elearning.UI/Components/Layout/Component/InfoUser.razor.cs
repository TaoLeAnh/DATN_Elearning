using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.AIM.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Elearning.UI.Components.Layout.Component
{
    public partial class InfoUser : ComponentBase
    {

        [Inject] protected ICallServiceRegistry CallServiceRegistry { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        //[Inject] private IUserService UserService { get; set; } = default!;
        [Inject] private ICallServiceRegistry CallService { get; set; } = default!;

        [CascadingParameter]
        public FluentDialog Dialog { get; set; } = default!;

        [CascadingParameter]
        protected CurrentUserDto CurrentUser { set; get; } = new();

        [Parameter]
        public Guid Content { get; set; }
        public string? ErrorMessage { get; set; } = string.Empty;
        public UserDto Item { get; set; } = new();



        protected override async Task OnInitializedAsync()
        {

            try
            {
                Item = await GetInforUser(Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API: {ex.Message}");
            }

        }

        public async Task<UserDto> GetInforUser(Guid? Id)
        {


            try
            {
                ApiRequestModel apiRequest = new ApiRequestModel()
                {

                    ApiService = ServicesRegistryEnum.ServicePortal,
                    Endpoint = $"/NguoiDung/{Id}",
                    HasAuthorization = true
                };

                var result = await CallService.Get<UserDto>(apiRequest);

                if (result.Status == StatusCode.OK)
                {
                    return result.Data as UserDto ?? throw new Exception("Dữ liệu trả về không đúng định dạng RoleDto.");
                }
                else
                {
                    throw new Exception(result.Message ?? "Lỗi khi lấy thông tin.");
                }
            }
            catch (Exception)
            {
                return new UserDto();
            }

        }

        private string GetVaiTroText(object? vaiTro)
        {
            if (vaiTro == null) return "Chưa xác định";

            var vt = vaiTro.ToString() ?? "";
            return vt switch
            {
                "0" => "Admin",
                "1" => "Giảng viên",
                "2" => "Sinh viên",
                _ => vt // Nếu API trả về chữ sẵn thì hiện luôn chữ
            };
        }
        public async Task CancelAsync()
        {
            await Dialog.CancelAsync();
        }



    }
}

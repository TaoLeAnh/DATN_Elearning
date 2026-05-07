using Elearning.Shared.Commons.Enums;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.Extentions.Redis;
using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Elearning.Shared.Contracts.AIM.Dtos;
using Elearning.UI.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Reflection;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace Elearning.UI.Components.Layout
{
    public partial class NavMenu
    {
        [Inject] private IModuleRegistry Registry { get; set; } = default!;
        [Inject] private ModuleTypeState TypeState { get; set; } = default!;
        [Inject] private ICallServiceRegistry CallService { get; set; } = default!;
        [Inject] private ICacheService CacheService { get; set; } = default!;

        [CascadingParameter] protected CurrentUserDto CurrentUser { set; get; } = new();
        private List<NavItem> NavMenuItems = new List<NavItem>();
        private Assembly assembly = typeof(Icons.Regular.Size20).Assembly;
        private bool IsLoadingDataMenu { get; set; } = true;
        protected override void OnInitialized()
        {



            TypeState.Changed += async () => await Refresh();
            _ = Refresh();

        }

        private List<ModuleTreeDto> FilterMenusByUser(
      List<ModuleTreeDto> allMenus,
      HashSet<Guid> allowedModuleIds)
        {
            var result = new List<ModuleTreeDto>();

            foreach (var menu in allMenus)
            {
                // Lọc children trước
                var filteredChildren = FilterMenusByUser(menu.Children, allowedModuleIds);

                // Giữ lại node nếu user có quyền, hoặc có con nào user có quyền
                if (allowedModuleIds.Contains(menu.Id) || filteredChildren.Any())
                {
                    result.Add(new ModuleTreeDto
                    {
                        Id = menu.Id,
                        ModuleChaId = menu.ModuleChaId,
                        TenModule = menu.TenModule,
                        Icon = menu.Icon,
                        LienKet = menu.LienKet,
                        Expands = menu.Expands,
                        ViTri = menu.ViTri,
                        PhanLoaiMenu = menu.PhanLoaiMenu,
                        PhanLoai = menu.PhanLoai,
                        Name = menu.Name,
                        ModerationStatus = menu.ModerationStatus,
                        Checked = menu.Checked,
                        Children = filteredChildren
                    });
                }
            }

            return result.OrderBy(m => m.ViTri).ToList();
        }
        private async Task Refresh()
        {
            try
            {
                ApiRequestModel apiRequest = new ApiRequestModel()
                {
                    ApiService = ServicesRegistryEnum.ServicePortal,
                    Endpoint = $"/Module/get-tree-publish?phanLoai={TypeState.Current}"
                };

                ResultAPI<List<ModuleTreeDto>> menus = await CallService.Get<List<ModuleTreeDto>>(apiRequest);
                if (CurrentUser.Menus != null && menus.Data != null)
                {
                    var allowedModuleIds = CurrentUser.Menus
                       .Select(m => m.Id)
                       .ToHashSet();
                    menus.Data = FilterMenusByUser(menus.Data, allowedModuleIds);
                }
                else
                {
                    menus.Data = new List<ModuleTreeDto>();
                }

                var navItems = BuildNavItems(menus.Data ?? new List<ModuleTreeDto>());
                NavMenuItems = navItems;

                // If no menu items returned from backend, fall back to local module registry
                if (NavMenuItems == null || !NavMenuItems.Any())
                {
                    try
                    {
                        var modules = Registry.GetByType(TypeState.Current);
                        var built = modules.SelectMany(m => m.BuildMenu(CurrentUser ?? new CurrentUserDto())).ToList();
                        if (built != null && built.Any())
                        {
                            NavMenuItems = built.ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("NavMenu fallback build error: " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception so we can see why NavMenu fails during development
                Console.Error.WriteLine("NavMenu.Refresh exception: " + ex.ToString());
                // Ensure UI doesn't stay stuck loading
                IsLoadingDataMenu = false;
                try { StateHasChanged(); } catch { }
                return;
            }

            //#region Build ra menu động tin bài
            //if (TypeState.Current == Service.Shared.Contracts.AIM.Enums.EnumModuleType.NghiepVu)
            //{
            //    List<WfTinBaiStatus> WorkFlowAllStatus =
            //        await CacheService.GetAsync<List<WfTinBaiStatus>>(RedisTypeKey.Core, RedisKeys.WorkFlowStatesWithActions)
            //        ?? new List<WfTinBaiStatus>();


            //    List<NavItem> menuTinTucDynamic = WorkFlowAllStatus
            //        .Where(x => x.Code != "DACONGKHAI" && x.Code != "DANGBIENTAP" && x.CreateMenu)
            //        .OrderBy(x => x.STT)
            //        .Select(item => new NavLink(
            //            $"/nghiep-vu/quan-ly-tin-tuc/bai-viet-dong/{item.Code}",
            //            GetIconByName(item.Icon),
            //            item.TenMenu ?? item.TenTrangThai,
            //            new List<EnumRoles>()
            //        )
            //        { STT = item.STT })
            //        .Cast<NavItem>()
            //        .ToList();
            //    var ItemMenuTinTuc = NavMenuItems
            //        .FirstOrDefault(x => x.Title.ToUpper() == "TIN TỨC");

            //    if (ItemMenuTinTuc is NavGroup group && group.Children != null)
            //    {
            //        group.Children = group.Children
            //            .Concat(menuTinTucDynamic)
            //            .OrderBy(x => x.STT)
            //            .ToList();
            //    }

            //}
            //#endregion




            IsLoadingDataMenu = false;
            StateHasChanged();
        }

        public void Dispose() => TypeState.Changed -= async () => await Refresh();


        private List<NavItem> BuildNavItems(List<ModuleTreeDto> modules)
        {
            var items = new List<NavItem>();

            foreach (var dto in modules.OrderBy(x => x.ViTri))
            {
                var icon = GetIconByName(dto.Icon);

                if (dto.Children != null && dto.Children.Any())
                {
                    items.Add(new NavGroup(
                        icon,
                        dto.TenModule,
                        dto.Expands,
                        "gap-sm",
                        BuildNavItems(dto.Children)
                    )
                    { STT = dto.ViTri }); // gán STT
                }
                else
                {
                    items.Add(new NavLink(
                        dto.LienKet,
                        icon,
                        dto.TenModule,
                        new List<EnumRoles>()
                    )
                    { STT = dto.ViTri }); // gán STT
                }
            }

            return items;
        }

        private Icon GetIconByName(string? name)
        {
            Type? type = assembly.GetType($"Microsoft.FluentUI.AspNetCore.Components.Icons.Regular.Size20+{name}");
            if (type is null)
                return new Icons.Regular.Size20.Circle();

            Icon? icon = (Icon?)Activator.CreateInstance(type);

            if (icon is null)
                return new Icons.Regular.Size20.Circle();

            return icon;
        }

    }

    public abstract record NavItem
    {

        public string Title { get; init; } = string.Empty;
        private string? _href;
        public string? Href
        {
            get => string.IsNullOrEmpty(_href)
                ? _href
                : _href!.StartsWith("/") ? _href[1..] : _href;
            init => _href = value;
        }
        public NavLinkMatch Match { get; init; } = NavLinkMatch.Prefix;
        public Icon Icon { get; init; } = new Icons.Regular.Size20.Document();
        public List<EnumRoles> RoleAppLy { set; get; } = new List<EnumRoles>();
        public int STT { get; set; } = 0;
    }

    public record NavLink : NavItem
    {
        public object? Tag { get; set; }
        public NavLink(string? href, Icon icon, string title, List<EnumRoles> roleAppLy, NavLinkMatch match = NavLinkMatch.Prefix)
        {
            Href = href;
            Icon = icon;
            Title = title;
            Match = match;
            RoleAppLy = roleAppLy;
        }
    }

    public record NavGroup : NavItem
    {
        public bool Expanded { get; set; }
        public string Gap { get; init; }
        public List<NavItem> Children { get; set; }

        public NavGroup(Icon icon, string title, bool expanded, string gap, List<NavItem> children)
        {
            Href = null;
            Icon = icon;
            Title = title;
            Expanded = expanded;
            Gap = gap;
            Children = children;
        }
    }
}

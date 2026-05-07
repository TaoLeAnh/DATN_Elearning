using Elearning.Shared.Commons.Enums;
using Elearning.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.AIM.Enums;
using Elearning.UI.Components.Layout;
using Microsoft.AspNetCore.Components.Routing;
using System.Reflection;
using NavLink = Elearning.UI.Components.Layout.NavLink;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace Elearning.UI.Application
{
    public interface INavModule
    {
        EnumModuleType Type { get; }

        IEnumerable<NavItem> BuildMenu(CurrentUserDto user);

        void RegisterServices(IServiceCollection services);

        Assembly[] GetRouteAssemblies();
    }

    public interface IModuleRegistry
    {
        IEnumerable<INavModule> All { get; }

        IEnumerable<INavModule> GetByType(EnumModuleType type);
    }

    public sealed class ModuleRegistry : IModuleRegistry
    {
        public IEnumerable<INavModule> All { get; }

        public ModuleRegistry(IEnumerable<INavModule> modules) => All = modules;

        public IEnumerable<INavModule> GetByType(EnumModuleType type) => All.Where(m => m.Type == type);
    }

    public sealed class AdminModule : INavModule
    {
        public EnumModuleType Type => EnumModuleType.HeThong;

        public IEnumerable<NavItem> BuildMenu(CurrentUserDto user)
        {
            var NavMenuItems = new List<NavItem>
            {
                // Trang làm việc
                new NavLink(
                    href: "/",
                    icon: new Icons.Regular.Size20.Home(),
                    title: "Bàn làm việc",
                    roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                ),
                new NavLink(
                    href: "/nghiep-vu/quan-ly-tin-tuc/bai-viet",
                    icon: new Icons.Regular.Size20.News(),
                    title: "Nghiệp vụ tin tức",
                    roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                ),

                // . Quản lý người dùng
                new NavGroup(
                    icon: new Icons.Regular.Size20.DataUsage(),
                    title: "Quản lý người dùng",
                    expanded: true,
                    gap: "0",
                    children: new List<NavItem>
                    {
                        new NavLink(
                            href: "/quan-tri-he-thong/nguoi-dung-va-he-thong/users",
                            icon: new Icons.Regular.Size20.ContactCard(),
                            title: "Quản lý người dùng",
                            roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                        ),
                        new NavLink(
                            href: "/quan-tri-he-thong/nguoi-dung-va-he-thong/co-cau-to-chuc",
                            icon: new Icons.Regular.Size20.Organization(),
                            title: "Quản lý nhóm người dùng",
                            roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                        ),
                        new NavLink(
                            href: "/quan-tri-he-thong/nguoi-dung-va-he-thong/roles",
                            icon: new Icons.Regular.Size20.ShieldLock(),
                            title: "Quản lý vai trò",
                            roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                        )
                        ,
                        new Components.Layout.NavLink(
                            href: "/quan-tri-he-thong/nguoi-dung-va-he-thong/permissions",
                            icon: new Icons.Regular.Size20.LockClosed(),
                            title: "Quản lý quyền",
                            roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                        ),

                        new Components.Layout.NavLink(
                            href: "/quan-tri-he-thong/nguoi-dung-va-he-thong/cau-hinh-nghiep-vu",
                            icon: new Icons.Regular.Size20.Wrench(),
                            title: "Cấu hình nghiệp vụ tin bài",
                            roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                        ),
                    }
                ),
                new NavGroup(
                    icon: new Icons.Regular.Size20.DataArea(),
                    title: "Quản lý hệ thống",
                    expanded: true,
                    gap: "0",
                    children: new List<NavItem>
                    {
                         new NavLink(
                            href: "/quan-tri-he-thong/quan-tri-menu",
                            icon: new Icons.Regular.Size20.Wrench(),
                            title: "Quản lý module",
                            roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                        ),
                        new NavLink(
                            href: "/quan-tri-he-thong/nguoi-dung-va-he-thong/tham-so-he-thong",
                            icon: new Icons.Regular.Size20.Wrench(),
                            title: "Cấu hình tham số PM",
                            roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                        ),
                        new NavLink(
                            href: "/quan-tri-he-thong/nguoi-dung-va-he-thong/cong-thanh-phan",
                            icon: new Icons.Regular.Size20.StoreMicrosoft(),
                            title: "Quản lý cổng thành phần",
                            roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                        ),
                         new NavGroup(
                                icon: new Icons.Regular.Size20.Apps(),
                                title: "Chuyên trang",
                                expanded: true,
                                gap: "0",
                                children: new List<NavItem>
                                {
                                    new NavLink(
                                    href: "/quan-tri-he-thong/chuyen-trang/chu-de-template",
                                    icon: new Icons.Regular.Size20.DesignIdeas(),
                                    title: "Quản lý chủ đề template",
                                    roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                                    ),
                                     new NavLink(
                                    href: "/quan-tri-he-thong/chuyen-trang/chuyen-trang-template",
                                    icon: new Icons.Regular.Size20.LayoutColumnFour(),
                                    title: "Quản lý chuyên trang template",
                                    roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                                    ),
                                     new NavLink(
                                    href: "/quan-tri-he-thong/chuyen-trang/trang-thong-tin",
                                    icon: new Icons.Regular.Size20.DocumentText(),
                                    title: "Quản lý trang thông tin",
                                    roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                                    ),
                                }
                            ),
                        new NavLink(
                            href: "/quan-tri-he-thong/nguoi-dung-va-he-thong/nhat-ky-he-thong",
                            icon: new Icons.Regular.Size20.History(),
                            title: "Quản lý log",
                            roleAppLy: new List<EnumRoles> { EnumRoles.QuanTriHeThong }
                        )
                    }
                )
            };
            return NavMenuItems;
        }

        public void RegisterServices(IServiceCollection services)
        { /* DI cho admin */ }

        public Assembly[] GetRouteAssemblies() => new[] { typeof(AdminModule).Assembly };
    }

    public sealed class NghiepVuModule : INavModule
    {
        public EnumModuleType Type => EnumModuleType.NghiepVu;

        public IEnumerable<NavItem> BuildMenu(CurrentUserDto user) => new[]
        {
        new NavGroup(new Icons.Regular.Size20.Document(), "Tin tức", true, "0", new List<NavItem>
        {
            new NavLink("/quan-ly-tin-tuc/bai-viet", new Icons.Regular.Size20.News(), "Bài viết",
                new(){ EnumRoles.QuanTriHeThong }),
            new NavLink("/quan-ly-tin-tuc/chuyen-muc", new Icons.Regular.Size20.Folder(), "Chuyên mục",
                new(){ EnumRoles.QuanTriHeThong }),
            new NavLink("/quan-tri-he-thong/danh-muc-dung-chung/su-kien-bai-viet", new Icons.Regular.Size20.Folder(), "Sự kiện bài viết",
                new(){ EnumRoles.QuanTriHeThong }),
        })
    };

        public void RegisterServices(IServiceCollection services)
        { /* DI cho tin tức */ }

        public Assembly[] GetRouteAssemblies() => new[] { typeof(NghiepVuModule).Assembly };
    }
}

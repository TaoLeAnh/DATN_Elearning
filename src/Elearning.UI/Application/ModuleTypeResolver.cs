using Elearning.Shared.Contracts.AIM.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Elearning.UI.Application
{
    public interface IModuleTypeResolver
    {
        EnumModuleType Resolve(string absoluteUri);
    }

    public sealed class DefaultModuleTypeResolver : IModuleTypeResolver
    {
        private readonly NavigationManager _nav;
        private readonly Dictionary<string, EnumModuleType> _map;

        public DefaultModuleTypeResolver(NavigationManager nav)
        {
            _nav = nav;
            _map = new(StringComparer.OrdinalIgnoreCase)
            {
                // map prefix thư mục đầu tiên
                ["ban-lam-viec"] = EnumModuleType.HeThong,
                ["quan-tri-he-thong"] = EnumModuleType.HeThong,
                ["nghiep-vu"] = EnumModuleType.NghiepVu,
                ["module"] = EnumModuleType.NghiepVu,
                ["module-khac"] = EnumModuleType.ModuleKhac,
                ["nghiep-vu-khac"] = EnumModuleType.NghiepVuKhac,
                // thêm ….
            };
        }

        public EnumModuleType Resolve(string absoluteUri)
        {
            var uri = new Uri(absoluteUri);
            var qs = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var overrideType = qs["type"] ?? qs["moduleType"]; // ?type=HeThong/NghiepVu/Khac
            if (!string.IsNullOrWhiteSpace(overrideType)
                && Enum.TryParse<EnumModuleType>(overrideType, true, out var t)) return t;

            var rel = _nav.ToBaseRelativePath(absoluteUri).Trim('/').ToLowerInvariant();
            var first = rel.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (first != null && _map.TryGetValue(first, out var typeFromPrefix)) return typeFromPrefix;

            return EnumModuleType.KhongXacDinh;
        }
    }




    public sealed class ModuleTypeState : IDisposable
    {
        private readonly NavigationManager _nav;
        private readonly IModuleTypeResolver _resolver;
        public EnumModuleType Current { get; private set; } = EnumModuleType.KhongXacDinh;
        public event Action? Changed;

        public ModuleTypeState(NavigationManager nav, IModuleTypeResolver resolver)
        {
            _nav = nav; _resolver = resolver;
            Set(_nav.Uri);
            _nav.LocationChanged += OnLocationChanged;
        }
        private void OnLocationChanged(object? s, LocationChangedEventArgs e) => Set(e.Location);
        private void Set(string uri)
        {
            var t = _resolver.Resolve(uri);
            if (t != Current) { Current = t; Changed?.Invoke(); }
        }
        public void Dispose() => _nav.LocationChanged -= OnLocationChanged;
    }
}

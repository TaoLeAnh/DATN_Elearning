using Elearning.Shared.Commons.Enums;
using System.Text.Json.Serialization;

namespace Elearning.Shared.Commons.Model.Commons
{
    public class CurrentUserDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        /// <summary>
        ///  Danh sách menu được phân
        /// </summary>
        [JsonPropertyName("menus")]
        public List<MenuItemDto>? Menus { get; set; }
        /// <summary>
        /// Phân quyền người dùng
        /// Phân quyền trên từng site
        /// </summary>
        public List<SitePermissionDto> PhanQuyen { get; set; } = new();



        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        public bool SupperUser { get; set; }




        // CôngVM: Cache lookup dictionary. supper fast
        private Dictionary<Guid, HashSet<EnumPermissions>>? _permissionCache;

        [JsonIgnore]
        public Dictionary<Guid, HashSet<EnumPermissions>> PermissionCache
        {
            get
            {
                if (_permissionCache == null)
                {
                    _permissionCache = new Dictionary<Guid, HashSet<EnumPermissions>>(PhanQuyen.Count);
                    foreach (var site in PhanQuyen)
                    {
                        if (site.Permissions != null && site.Permissions.Count > 0)
                        {
                            var permSet = new HashSet<EnumPermissions>(site.Permissions.Count);
                            foreach (var p in site.Permissions)
                                permSet.Add(p.PermissionCode);

                            _permissionCache[site.SiteId] = permSet;
                        }
                    }
                }
                return _permissionCache;
            }
        }

        // Fast permission check - O(1)
        public bool HasPermission(Guid siteId, EnumPermissions permission)
        {
            if (SupperUser) return true;

            if (PermissionCache.TryGetValue(siteId, out var permissions))
            {
                if (permissions.Contains(EnumPermissions.SIEUQUANTRI))
                {
                    return true;
                }
                return permissions.Contains(permission);
            }
            return false;

        }
        //có ít nhất 1 trong danh sách quyền
        public bool HasAnyPermission(Guid siteId, List<EnumPermissions> permissionsToCheck)
        {
            if (SupperUser) return true;

            if (permissionsToCheck == null || permissionsToCheck.Count == 0) return false;

            if (PermissionCache.TryGetValue(siteId, out var userPermissions))
            {
                if (userPermissions.Contains(EnumPermissions.SIEUQUANTRI))
                {
                    return true;
                }
                return permissionsToCheck.Any(p => userPermissions.Contains(p));
            }
            return false;
        }
        public Guid GetIdSiteDefault()
        {
            var site = PhanQuyen.Where(x => x.DefaultSelect).FirstOrDefault();
            if (site is null)
                return Guid.Empty;
            return site.SiteId;
        }


    }
}

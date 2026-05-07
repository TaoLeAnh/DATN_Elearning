using Elearning.Shared.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.Commons
{
    public class SitePermissionDto
    {


        /// <summary>
        ///Nếu là true thì sẽ hiểu là đây là chuyên trang gốc của cổng
        /// </summary>
        public bool DefaultSelect { get; set; }

        /// <summary>
        /// Id chuyên trang
        /// </summary>
        public Guid SiteId { get; set; }
        //Tên chuyên trang
        public string SiteName { get; set; } = string.Empty;
        /// <summary>
        /// Danh sách chuyên mục được phân quyền
        /// </summary>
        public List<CategoryPermissionDto>? Categories { get; set; }
        /// <summary>
        /// Danh sách vai trò được phân quyền
        /// </summary>
        public List<RolePermissionDto>? Roles { get; set; }
        /// <summary>
        /// Danh sách quyền được phân
        /// </summary>
        public List<Permission>? Permissions { get; set; }

    }

    public class CategoryPermissionDto
    {
        public Guid CategoryId { get; set; }
    }

    public class RolePermissionDto
    {
        public Guid RoleId { get; set; }
        public string RoldeCode { get; set; } = string.Empty;
    }

    public class Permission
    {
        public string Name { get; set; } = string.Empty;
        public EnumPermissions PermissionCode { set; get; }
    }

    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

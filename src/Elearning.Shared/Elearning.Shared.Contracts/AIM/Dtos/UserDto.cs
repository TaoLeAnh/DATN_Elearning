using Elearning.Shared.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class UserDto : BaseEntiyDto
    {
        public int Index { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public object VaiTro { get; set; } = default!;
        public string FullName { get; set; } = string.Empty;

        // Thông tin xác thực
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public bool PhoneNumberConfirmed { get; set; } = false;
        public DateTime? LockoutEnd { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? LastLoginIp { get; set; }
        public string? AvatarUrl { get; set; }
        public int TotalLogin { get; set; }
        public int TotalLoginFaild { get; set; }
        public Guid? GroupID { get; set; }
        public Guid? PhongBanId { get; set; }

        public string VaiTrotxt { get; set; } = string.Empty;
        public string DonVitxt { get; set; } = string.Empty;
        public string PhongBanTxt { get; set; } = string.Empty;

        public List<UserRoleHistoryDto>? UserRoleHistory { get; set; }
    }
    public class CopyUserInfoDto
    {
        public Guid SourceUserId { get; set; }
        public Guid TargetUserId { get; set; }
    }
}

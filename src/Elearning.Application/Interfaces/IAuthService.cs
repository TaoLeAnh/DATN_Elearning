using Elearning.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface IAuthService
    {
        Task<NguoiDung> LoginAsync(string email, string password);

        // Sau này bạn có thể ném thêm các hàm như ChangePasswordAsync, ResetPasswordAsync vào đây
    }
}

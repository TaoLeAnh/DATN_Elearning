using Elearning.Application.Interfaces;
using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Elearning.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<NguoiDung> LoginAsync(string email, string password)
        {
            // 1. Tìm user theo Email
            var user = await _unitOfWork.NguoiDungRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                return null;
            }

            // 2. Dùng BCrypt kiểm tra mật khẩu
            var hasher = new BCryptPasswordHasher();
            bool isPasswordValid = hasher.Verify(password, user.MatKhau);

            if (isPasswordValid)
            {
                return user;
            }

            return null;
        }
    }
}

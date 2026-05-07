using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces;
using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Microsoft.EntityFrameworkCore;

namespace Elearning.Publising.Application.Services
{
    public class AuthPublisingService : IAuthPublisingService
    {
        private readonly IUnitOfWorkPublising _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public AuthPublisingService(IUnitOfWorkPublising unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> CheckEmailExistAsync(string email)
        {
            // Kiểm tra email có trong hệ thống chưa
            return await _unitOfWork.NguoiDungRepository.GetTableNoTracking()
                                    .AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<NguoiDung?> RegisterAsync(RegisterRequest request)
        {
            // Kiểm tra lại lần nữa cho chắc ăn
            if (await CheckEmailExistAsync(request.Email)) return null;

            var newUser = new NguoiDung
            {
                Email = request.Email,
                MatKhau = _passwordHasher.Hash(request.Password),
                Ten = "Học viên mới",
                VaiTro = EnumVaiTro.HocSinh
            };

            await _unitOfWork.NguoiDungRepository.AddAsync(newUser);
            await _unitOfWork.CompleteAsync(newUser.Id); // Lưu xuống DB

            return newUser;
        }

        public async Task<NguoiDung?> LoginAsync(LoginRequest request)
        {
            // BƯỚC 1: Chỉ tìm user theo Email dưới Database
            var user = await _unitOfWork.NguoiDungRepository.GetTableNoTracking()
                                    .FirstOrDefaultAsync(x => x.Email.ToLower() == request.Email.ToLower());

            // Nếu không tìm thấy email trong hệ thống -> Trả về null ngay
            if (user == null) return null;

            // BƯỚC 2: Xài hàm Verify của BCrypt để so sánh mật khẩu (Chạy bằng C#)
            bool isPasswordValid = _passwordHasher.Verify(request.Password, user.MatKhau);

            // Nếu mật khẩu sai -> Trả về null
            if (!isPasswordValid) return null;

            // Vượt qua vòng bảo vệ thì trả về thông tin User
            return user;
        }
    }
}

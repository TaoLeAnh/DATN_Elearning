using Elearning.Domain.Entities;
using Elearning.Shared.Contracts.Portal.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Interfaces
{
    public interface IAuthPublisingService
    {
        Task<bool> CheckEmailExistAsync(string email);
        Task<NguoiDung?> RegisterAsync(RegisterRequest request);
        Task<NguoiDung?> LoginAsync(LoginRequest request);
    }
}

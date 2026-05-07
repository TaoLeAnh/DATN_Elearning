using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Interfaces
{
    public interface IHoSoGiaoVienService
    {
        // Lấy danh sách giáo viên (có hỗ trợ lọc theo môn học để bấm vào các Tab)
        Task<List<HoSoGiaoVienDto>> GetDanhSachGiaoVienAsync(MonHocEnum? monHoc = null);

        // Lấy chi tiết 1 giáo viên
        Task<HoSoGiaoVienDto?> GetChiTietGiaoVienAsync(Guid id);
    }
}

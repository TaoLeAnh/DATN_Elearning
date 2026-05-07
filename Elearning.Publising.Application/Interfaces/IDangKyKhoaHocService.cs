using Elearning.Shared.Contracts.Portal.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Interfaces
{
    public interface IDangKyKhoaHocService
    {
        Task<List<MyCourseDto>> GetMyCoursesAsync(Guid userId);
        Task<string> DangKyKhoaHocMoiAsync(Guid userId, Guid khoaHocId);
        Task<int> CountTatCaHocVienAsync();
    }
}

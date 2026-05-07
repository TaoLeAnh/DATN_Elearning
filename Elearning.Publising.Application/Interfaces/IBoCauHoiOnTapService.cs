using Elearning.Shared.Contracts.Portal.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Interfaces
{
    public interface IBoCauHoiOnTapService
    {
        Task<BoCauHoiOnTapDto> GetQuizDetailForStudentAsync(Guid id);
        Task<float> NopBaiVaChamDiemAsync(NopBaiRequest request, Guid userId);
    }
}

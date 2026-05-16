using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Interfaces
{
    public interface IKyThiService
    {
        Task<List<PublicKyThiDto>> GetPublicExamsAsync(MonHocEnum? monHoc = null);
        Task<Guid> GenerateRandomExamAsync(MonHocEnum monHoc);

        Task<BoCauHoiOnTapDto> GetDeThiLamBaiAsync(Guid kyThiId);
        Task<float> NopBaiThiAsync(NopBaiRequest payload);

        Task<Guid> BatDauThiAsync(Guid kyThiId, Guid userId);
        Task<bool> GhiNhanViPhamRealTimeAsync(Guid baiLamId, EnumLoaiViPham loai, string chiTiet);
        Task<(bool, string)> DayBaiNopVaoQueueAsync(NopBaiRequest request);
        Task<List<BaiLamDto>> GetMyExamsAsync(Guid userId);
        Task<BaiLamReviewDto> GetChiTietBaiLamHocVienAsync(Guid baiLamId, Guid userId);
    }
}

using Elearning.Domain.Entities;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Interfaces.MSSQL
{
    public interface IHoSoGiaoVienRepository : IRepository<HoSoGiaoVien>
    {
        Task<(List<HoSoGiaoVienDto> Items, int Total)> GetPagedDtoAsync(HoSoGiaoVienQuery searchOption);
        Task<List<HoSoGiaoVienDto>> GetDanhSachGiaoVienPubAsync(MonHocEnum? monHoc = null);
    }
}

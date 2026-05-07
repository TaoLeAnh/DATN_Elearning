using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Shared.Commons.Interfaces.SQL;

namespace Elearning.Domain.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        INguoiDungRepository NguoiDungRepository { get; }
        IKhoaHocRepository KhoaHocRepository { get; }

        IChuongHocRepository ChuongHocRepository { get; }
        IBaiHocRepository BaiHocRepository { get; }
        IDangKyKhoaHocRepository DangKyKhoaHocRepository { get; }

        ITienDoHocRepository TienDoHocRepository { get; }

        ICauHoiRepository CauHoiRepository { get; }

        IBoCauHoiOnTapRepository BoCauHoiOnTapRepository { get; }

        IChiTietBoCauHoiRepository ChiTietBoCauHoiRepository { get; }

        IKyThiRepository KyThiRepository { get;  }

        ICauHoiKyThiRepository CauHoiKyThiRepository { get; }

        IBaiLamRepository BaiLamRepository { get; }

        IChiTietBaiLamRepository ChiTietBaiLamRepository { get; }

        IDapAnRepository DapAnRepository { get; }
        
        ILogViPhamRepository LogViPhamRepository { get; }

        IMaTranDeThiMacDinhRepository MaTranDeThiMacDinhRepository { get; }

        IChiTietMaTranMacDinhRepository ChiTietMaTranMacDinhRepository { get; }

        IHoSoGiaoVienRepository HoSoGiaoVienRepository { get; } 
    }
}

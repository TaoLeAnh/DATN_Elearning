using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Shared.Commons.Interfaces.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Interfaces
{
    public interface IUnitOfWorkPublising : IBaseUnitOfWork
    {
        public IKhoaHocRepository KhoaHocRepository { get; }
        
        public IBoCauHoiOnTapRepository BoCauHoiOnTapRepository { get; }
        public IBaiLamRepository BaiLamRepository { get; }

        public INguoiDungRepository NguoiDungRepository { get; }

        public IKyThiRepository KyThiRepository { get; }
        public ICauHoiRepository CauHoiRepository { get; }
        public ICauHoiKyThiRepository CauHoiKyThiRepository { get; }

        public ILogViPhamRepository LogViPhamRepository { get; }

        public ITienDoHocRepository TienDoHocRepository { get; }
        public IChuongHocRepository ChuongHocRepository { get; }

        public IMaTranDeThiMacDinhRepository MaTranDeThiMacDinhRepository { get; }

        public IHoSoGiaoVienRepository HoSoGiaoVienRepository { get; }

        public IDangKyKhoaHocRepository DangKyKhoaHocRepository { get; }

        public IChiTietBaiLamRepository ChiTietBaiLamRepository { get; }
        public IBaiHocRepository BaiHocRepository { get; }
    }
}

using Elearning.Domain.Interfaces;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Repository;
using Elearning.Publising.Infrastructure.Persistence.Context;
using Elearning.Publising.Infrastructure.Repositories.Bases;
using Elearning.Shared.Commons.Interfaces.Extentions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Infrastructure.Repositories
{
    public class UnitOfWorkPublising : BaseUnitOfWork, IUnitOfWorkPublising
    {
        private readonly AppDbContext _context;

        public IKhoaHocRepository? _khoaHocRepository;
        public IBoCauHoiOnTapRepository? _boCauHoiOnTapRepository;
        public IBaiLamRepository? _baiLamRepository;
        public INguoiDungRepository? _nguoiDungRepository;
        public IKyThiRepository? _kyThiRepository;
        public ICauHoiRepository? _cauHoiRepository;
        public ICauHoiKyThiRepository? _cauHoiKyThiRepository;
        public ILogViPhamRepository? _logViPhamRepository;
        public ITienDoHocRepository? _tienDoHocRepository;
        public IChuongHocRepository? _chuongHocRepository;
        public IMaTranDeThiMacDinhRepository? _maTranDeThiMacDinhRepository;
        public IHoSoGiaoVienRepository? _hoSoGiaoVienRepository;
        public IDangKyKhoaHocRepository? _dangKyKhoaHocRepository;
        public IChiTietBaiLamRepository? _chiTietBaiLamRepository;
        public IBaiHocRepository? _baiHocRepository;
        public UnitOfWorkPublising(AppDbContext context, IRequestContext requestContext)
           : base(context, requestContext)
        {
            _context = context;
        }

        public IKhoaHocRepository KhoaHocRepository =>
            _khoaHocRepository ??= new KhoaHocRepository(_context);

        public IBoCauHoiOnTapRepository BoCauHoiOnTapRepository =>
            _boCauHoiOnTapRepository ??= new BoCauHoiOnTapRepository(_context);

        public IBaiLamRepository BaiLamRepository =>
            _baiLamRepository ??= new BaiLamRepository(_context);

        public INguoiDungRepository NguoiDungRepository =>
            _nguoiDungRepository ??= new NguoiDungRepository(_context);

        public IKyThiRepository KyThiRepository =>
            _kyThiRepository ??= new KyThiRepository(_context);

        public ICauHoiRepository CauHoiRepository =>
            _cauHoiRepository ??= new CauHoiRepository(_context);

        public ICauHoiKyThiRepository CauHoiKyThiRepository =>
            _cauHoiKyThiRepository ??= new CauHoiKyThiRepository(_context);

        public ILogViPhamRepository LogViPhamRepository =>
            _logViPhamRepository ??= new LogViPhamRepository(_context);

        public ITienDoHocRepository TienDoHocRepository =>
            _tienDoHocRepository ??= new TienDoHocRepository(_context);

        public IChuongHocRepository ChuongHocRepository =>
            _chuongHocRepository ??= new ChuongHocRepository(_context);

        public IMaTranDeThiMacDinhRepository MaTranDeThiMacDinhRepository =>
            _maTranDeThiMacDinhRepository ??= new MaTranDeThiMacDinhRepository(_context);

        public IHoSoGiaoVienRepository HoSoGiaoVienRepository =>
            _hoSoGiaoVienRepository ??= new HoSoGiaoVienRepository(_context);

        public IDangKyKhoaHocRepository DangKyKhoaHocRepository =>
            _dangKyKhoaHocRepository ??= new DangKyKhoaHocRepository(_context);

        public IChiTietBaiLamRepository ChiTietBaiLamRepository =>
            _chiTietBaiLamRepository ??= new ChiTietBaiLamRepository(_context);

        public IBaiHocRepository BaiHocRepository =>
            _baiHocRepository ??= new BaiHocRepository(_context);
    }
}

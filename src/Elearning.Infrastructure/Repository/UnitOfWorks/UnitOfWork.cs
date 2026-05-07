using Elearning.Domain.Interfaces;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using Elearning.Shared.Commons.Interfaces.Extentions;

namespace Elearning.Infrastructure.Repository.UnitOfWorks
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        private readonly ElearningDbContext _context;

        private INguoiDungRepository? _nguoiDungRepository;

        private IKhoaHocRepository? _khoaHocRepository;

        private IChuongHocRepository? _chuongHocRepository;

        private IBaiHocRepository? _baiHocRepository;

        private IDangKyKhoaHocRepository? _dangKyKhoaHocRepository;

        private ITienDoHocRepository? _tienDoHocRepository;

        private ICauHoiRepository? _cauHoiRepository;

        private IBoCauHoiOnTapRepository? _boCauHoiOnTapRepository;

        private IChiTietBoCauHoiRepository? _chiTietBoCauHoiRepository;

        private IKyThiRepository? _kyThiRepository;

        private ICauHoiKyThiRepository? _cauHoiKyThiRepository;

        private IBaiLamRepository? _baiLamRepository;

        private IChiTietBaiLamRepository? _chiTietBaiLamRepository;

        private IDapAnRepository? _dapAnRepository;

        private ILogViPhamRepository? _logViPhamRepository;

        private IMaTranDeThiMacDinhRepository? _maTranDeThiMacDinhRepository;

        private IChiTietMaTranMacDinhRepository? _chiTietMaTranMacDinhRepository;

        private IHoSoGiaoVienRepository? _hoSoGiaoVienRepository;


        public UnitOfWork(ElearningDbContext context, IRequestContext requestContext)
          : base(context, requestContext)
        {
            _context = context;
        }

        public IKhoaHocRepository KhoaHocRepository
            => _khoaHocRepository ??= new KhoaHocRepository(_context);

        public IChuongHocRepository ChuongHocRepository
            => _chuongHocRepository ??= new ChuongHocRepository(_context);

        public INguoiDungRepository NguoiDungRepository
            => _nguoiDungRepository ??= new NguoiDungRepository(_context);

        public IBaiHocRepository BaiHocRepository
            => _baiHocRepository ??= new BaiHocRepository(_context);

        public IDangKyKhoaHocRepository DangKyKhoaHocRepository
            => _dangKyKhoaHocRepository ??= new DangKyKhoaHocRepository(_context);

        public ITienDoHocRepository TienDoHocRepository
            => _tienDoHocRepository ??= new TienDoHocRepository(_context);

        public ICauHoiRepository CauHoiRepository
            => _cauHoiRepository ??= new CauHoiRepository(_context);

        public IBoCauHoiOnTapRepository BoCauHoiOnTapRepository
            => _boCauHoiOnTapRepository ??= new BoCauHoiOnTapRepository(_context);

        public IChiTietBoCauHoiRepository ChiTietBoCauHoiRepository
            => _chiTietBoCauHoiRepository ??= new ChiTietBoCauHoiRepository(_context);

        public IKyThiRepository KyThiRepository
            => _kyThiRepository ??= new KyThiRepository(_context);

        public ICauHoiKyThiRepository CauHoiKyThiRepository
            => _cauHoiKyThiRepository ??= new CauHoiKyThiRepository(_context);

        public IBaiLamRepository BaiLamRepository
            => _baiLamRepository ??= new BaiLamRepository(_context);

        public IChiTietBaiLamRepository ChiTietBaiLamRepository
            => _chiTietBaiLamRepository ??= new ChiTietBaiLamRepository(_context);

        public IDapAnRepository DapAnRepository
            => _dapAnRepository ??= new DapAnRepository(_context);

        public ILogViPhamRepository LogViPhamRepository
            => _logViPhamRepository ??= new LogViPhamRepository(_context);

        public IMaTranDeThiMacDinhRepository MaTranDeThiMacDinhRepository
            => _maTranDeThiMacDinhRepository ??= new MaTranDeThiMacDinhRepository(_context);

        public IChiTietMaTranMacDinhRepository ChiTietMaTranMacDinhRepository
            => _chiTietMaTranMacDinhRepository ??= new ChiTietMaTranMacDinhRepository(_context);

        public IHoSoGiaoVienRepository HoSoGiaoVienRepository
            => _hoSoGiaoVienRepository ??= new HoSoGiaoVienRepository(_context);
    }
}

using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Publising.Infrastructure.Persistence.Context;
using Elearning.Publising.Infrastructure.Repositories.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Infrastructure.Repositories
{

    public class ChiTietBaiLamRepository : Repository<ChiTietBaiLam>, IChiTietBaiLamRepository
    {
        private readonly AppDbContext _context;

        public ChiTietBaiLamRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

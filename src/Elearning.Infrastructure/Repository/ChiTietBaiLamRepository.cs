using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Repository
{
    public class ChiTietBaiLamRepository : Repository<ChiTietBaiLam>, IChiTietBaiLamRepository
    {
        public ChiTietBaiLamRepository(ElearningDbContext context) : base(context)
        {
        }

    }
}

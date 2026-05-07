using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Publising.Infrastructure.Persistence.Context;
using Elearning.Publising.Infrastructure.Repositories.Bases;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Querys;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Infrastructure.Repositories
{
    public class BaiLamRepository : Repository<BaiLam>, IBaiLamRepository
    {
        private readonly AppDbContext _context;

        public BaiLamRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<(List<BaiLamDto> Items, int Total)> GetPagedDtoAsync(BaiLamQuery searchOption)
        {
            throw new NotImplementedException();
        }
    }
}

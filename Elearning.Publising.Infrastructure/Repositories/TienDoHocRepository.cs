using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Publising.Infrastructure.Persistence.Context;
using Elearning.Publising.Infrastructure.Repositories.Bases;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Infrastructure.Repositories
{
    public class TienDoHocRepository : Repository<TienDoHoc>, ITienDoHocRepository
    {
        private readonly AppDbContext _context;

        public TienDoHocRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public Task<(List<TienDoHocDto> Items, int Total)> GetPagedDtoAsync(TienDoHocQuery searchOption)
        {
            throw new NotImplementedException();
        }
    }
}

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
    public class BoCauHoiOnTapRepository : Repository<BoCauHoiOnTap>, IBoCauHoiOnTapRepository
    {
        private readonly AppDbContext _context;

        public BoCauHoiOnTapRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<(List<BoCauHoiOnTapDto> Items, int Total)> GetPagedDtoAsync(BoCauHoiOnTapQuery searchOption)
        {
            throw new NotImplementedException();
        }
    }
}

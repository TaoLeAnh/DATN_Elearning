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
    public class CauHoiKyThiRepository : Repository<CauHoiKyThi>, ICauHoiKyThiRepository
    {
        private readonly AppDbContext _context;

        public CauHoiKyThiRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

    }
}

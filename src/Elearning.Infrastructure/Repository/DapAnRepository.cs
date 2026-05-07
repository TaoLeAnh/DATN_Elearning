using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Infrastructure.Repository
{
    public class DapAnRepository : Repository<DapAn>, IDapAnRepository
    {
        public DapAnRepository(ElearningDbContext context) : base(context)
        {
        }

    }
}

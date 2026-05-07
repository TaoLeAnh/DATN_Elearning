using Elearning.Domain.Entities;
using Elearning.Domain.Interfaces.MSSQL;
using Elearning.Infrastructure.Persistence.Contexts;
using Elearning.Infrastructure.Repository.Base;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.EntityFrameworkCore;

namespace Elearning.Infrastructure.Repository
{
    public class NguoiDungRepository : Repository<NguoiDung>, INguoiDungRepository
    {
        private readonly ElearningDbContext _context;

        public NguoiDungRepository(ElearningDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<NguoiDungDto> Items, int Total)> GetPagedDtoAsync(NguoiDungQuery searchOption)
        {
            int total = 0;
            var baseQuery = FilterData(
                q => q.Where(x => !searchOption.isgetBylisID
                                  || searchOption.lstIDGet.Contains(x.Id)),
                searchOption.gridRequest,
                ref total);

            var dtoQuery = baseQuery.Select(x => new NguoiDungDto
            {
                Id = x.Id,
                Ten = x.Ten,
                Email = x.Email,
                VaiTro = x.VaiTro,
                Created = x.Created,
                LastModified = x.LastModified
            });

            var items = await dtoQuery.ToListAsync();
            return (items, total);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using GoPass.Domain.Models;
using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.IQueryableExtensions;

namespace GoPass.Infrastructure.Repositories.Classes;
public class ResaleRepository : GenericRepository<Resale>, IResaleRepository
{
    public ResaleRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<Resale> Publish(Resale reventa)
    {
        await _dbContext.AddAsync(reventa);

        return reventa;
    }

    public async Task<Resale> GetResaleByTicketId(int ticketId)
    {
        var resale = await _dbSet.Where(x => x.TicketId == ticketId).FirstOrDefaultAsync();

        if (resale is null) throw new Exception();

        return resale;
    }

    public override async Task<List<Resale>> GetAllWithPagination(PaginationDto paginationDto)
    {
        var recordsQueriable = _dbSet.AsQueryable();

        return await recordsQueriable.Paginate(paginationDto).Include(x => x.Ticket).ToListAsync();
    }
}

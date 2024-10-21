using Microsoft.EntityFrameworkCore;
using GoPass.Domain.Models;
using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.IQueryableExtensions;

namespace GoPass.Infrastructure.Repositories.Classes;
public class ReventaRepository : GenericRepository<Reventa>, IReventaRepository
{
    public ReventaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<Reventa> Publish(Reventa reventa, int vendedorId)
    {
        reventa.VendedorId = vendedorId;
        await _dbContext.AddAsync(reventa);

        return reventa;
    }

    public async Task<Reventa> GetResaleByEntradaId(int entradaId)
    {
        var resale = await _dbSet.Where(x => x.EntradaId == entradaId).SingleOrDefaultAsync();

        if (resale is null) throw new Exception();

        return resale;
    }

    public override async Task<List<Reventa>> GetAllWithPagination(PaginationDto paginationDto)
    {
        var recordsQueriable = _dbSet.AsQueryable();

        return await recordsQueriable.Paginate(paginationDto).Include(x => x.Entrada).ToListAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GoPass.Infrastructure.Data;
using GoPass.Domain.Models;
using GoPass.Infrastructure.Repositories.Interfaces;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.IQueryableExtensions;

namespace GoPass.Infrastructure.Repositories.Classes;
public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
{
    protected DbSet<T> _dbSet;
    protected DbContext _dbContext;
    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
        _dbContext = dbContext;
    }
    public async Task<List<T>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<List<T>> GetAllWithPagination(PaginationDto paginationDto)
    {
        var recordsQueriable = _dbSet.AsQueryable();

        return await recordsQueriable.Paginate(paginationDto).ToListAsync();
    }

    public async Task<T> GetById(int id)
    {
        var recordDb = await _dbSet.FindAsync(id);
        return recordDb!;
    }

    public async Task<T> Create(T model, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(model, cancellationToken);
        return model;
    }
    public virtual async Task<T> Update(int id, T model)
    {
        var existingData = await _dbSet.FindAsync(id);

        if (existingData is null) throw new KeyNotFoundException("No se encontro el modelo");

        _dbContext.Entry(existingData).CurrentValues.SetValues(model);

        return existingData;
    }

    public async Task<T> Delete(int id)
    {
        var recordToDelete = await GetById(id);

        if (recordToDelete is null) throw new Exception("El registro no se encontro");

        await _dbSet.Where(x => x.Id == id).ExecuteDeleteAsync();

        return recordToDelete;
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).FirstOrDefaultAsync();
}

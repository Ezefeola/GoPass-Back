using System.Linq.Expressions;
using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.Models;
using GoPass.Infrastructure.Repositories.Interfaces;

namespace GoPass.Application.Services.Classes;

public class GenericService<T> : IGenericService<T> where T : BaseModel
{
    protected readonly IGenericRepository<T> _genericRepository;

    public GenericService(IGenericRepository<T> genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _genericRepository.GetAll();
    }
    public async Task<List<T>> GetAllWithPaginationAsync(PaginationDto paginationDto)
    {
        var dbRecords = await _genericRepository.GetAllWithPagination(paginationDto);

        return dbRecords;
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _genericRepository.GetById(id);
    }

    public async Task<T> Create(T model)
    {
        return await _genericRepository.Create(model);
    }
    public async Task<T> Update(int id, T model)
    {
        return await _genericRepository.Update(id, model);
    }

    public async Task<T> Delete(int id)
    {
        return await _genericRepository.Delete(id);
    }

    public async Task<T> FindAsync(Expression<Func<T, bool>> predicate) => await _genericRepository.FindAsync(predicate);
}

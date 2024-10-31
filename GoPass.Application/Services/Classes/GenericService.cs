using System.Linq.Expressions;
using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.Models;
using GoPass.Infrastructure.Repositories.Interfaces;
using GoPass.Infrastructure.UnitOfWork;

namespace GoPass.Application.Services.Classes;

public class GenericService<T> : IGenericService<T> where T : BaseModel
{
    protected readonly IGenericRepository<T> _genericRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenericService(IGenericRepository<T> genericRepository, IUnitOfWork unitOfWork)
    {
        _genericRepository = genericRepository;
        _unitOfWork = unitOfWork;
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

    public async Task<T> CreateAsync(T model, CancellationToken cancellationToken)
    {
        var createdRecord = await _genericRepository.Create(model, cancellationToken);

        await _unitOfWork.Complete(cancellationToken);

        return createdRecord;
    }
    public virtual async Task<T> UpdateAsync(int id, T model, CancellationToken cancellationToken)
    {
        var recordUpdated = await _genericRepository.Update(id, model);

        await _unitOfWork.Complete(cancellationToken);

        return recordUpdated;
    }

    public async Task<T> DeleteAsync(int id)
    {
        var deletedRecord = await _genericRepository.Delete(id);

        return deletedRecord;
    }

    public async Task<T> FindAsync(Expression<Func<T, bool>> predicate) => await _genericRepository.FindAsync(predicate);
}

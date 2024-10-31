using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.Models;
using System.Linq.Expressions;

namespace GoPass.Application.Services.Interfaces;

public interface IGenericService<T> where T : BaseModel
{
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetAllWithPaginationAsync(PaginationDto paginationDto);
    Task<T> GetByIdAsync(int id);
    Task<T> CreateAsync(T model, CancellationToken cancellationToken);
    Task<T> UpdateAsync(int id, T model, CancellationToken cancellationToken);
    Task<T> DeleteAsync(int id);
    Task<T> FindAsync(Expression<Func<T, bool>> predicate);
}

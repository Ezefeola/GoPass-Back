using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.Models;
using System.Linq.Expressions;

namespace GoPass.Application.Services.Interfaces;

public interface IGenericService<T> where T : BaseModel
{
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetAllWithPaginationAsync(PaginationDto paginationDto);
    Task<T> GetByIdAsync(int id);
    Task<T> Create(T model);
    Task<T> Update(int id, T model);
    Task<T> Delete(int id);
    Task<T> FindAsync(Expression<Func<T, bool>> predicate);
}

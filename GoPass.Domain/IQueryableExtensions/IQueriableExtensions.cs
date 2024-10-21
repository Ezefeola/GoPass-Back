using GoPass.Domain.DTOs.Request.PaginationDTOs;

namespace GoPass.Domain.IQueryableExtensions;

public static class IQueriableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDto paginationDto)
    {
        return queryable.Skip((paginationDto.PageNumber - 1) * paginationDto.PageSize)
            .Take(paginationDto.PageSize);  
    }
}

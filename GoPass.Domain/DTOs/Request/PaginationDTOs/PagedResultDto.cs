namespace GoPass.Domain.DTOs.Request.PaginationDTOs;

public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
}

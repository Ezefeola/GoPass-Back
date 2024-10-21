using FluentValidation;
using GoPass.Domain.DTOs.Request.PaginationDTOs;

namespace GoPass.Application.Validators.Users;

public class PaginationDTOValidator : AbstractValidator<PaginationDto>
{
    public PaginationDTOValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("El numero de pagina debe ser mayor que 0");
        RuleFor(x => x.PageSize).InclusiveBetween(1,100).WithMessage("El tamaño de la pagina debe estar entre 1 y 100");
    }
}

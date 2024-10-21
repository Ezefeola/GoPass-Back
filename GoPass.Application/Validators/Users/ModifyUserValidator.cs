using FluentValidation;
using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.AuthRequestDTOs;

namespace GoPass.Application.Validators.Users;

public class ModifyUserValidator : AbstractValidator<ModifyUsuarioRequestDto>
{
    public ModifyUserValidator(IUsuarioService usuarioService)
    {

        RuleFor(u => u.DNI)
        .NotEmpty().WithMessage("El campo Dni es obligatorio")
        .MaximumLength(10).WithMessage("El campo {PropertyName} puede tener un maximo de 10 caracteres")
        .Matches(@"^\d+$").WithMessage("El campo {PropertyName} solo puede contener numeros");

        RuleFor(u => u.NumeroTelefono)
            .NotEmpty().WithMessage("El campo Numero de telefono es obligatorio")
            .MaximumLength(14).WithMessage("El campo {PropertyName} puede tener un maximo de 14 caracteres")
            .Matches(@"^\d+$").WithMessage("El campo {PropertyName} solo puede contener numeros");

        RuleFor(u => u.Nombre)
            .NotEmpty().WithMessage("El campo es obligatorio")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El campo {PropertyName} solo puede contener letras")
            .MinimumLength(2).MaximumLength(200).WithMessage("El campo {PropertyName} debe tener un minimo de 2 caracteres");
    }
}

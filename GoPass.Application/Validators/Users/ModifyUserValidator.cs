using FluentValidation;
using GoPass.Application.Services.Validations.Interfaces;
using GoPass.Domain.DTOs.Request.AuthRequestDTOs;

namespace GoPass.Application.Validators.Users;

public class ModifyUserValidator : AbstractValidator<ModifyUsuarioRequestDto>
{
    public ModifyUserValidator(IUserValidationService userValidationService)
    {

        RuleFor(u => u.DNI)
        .NotEmpty().WithMessage("El campo Dni es obligatorio")
        .MustAsync(async(dni, cancellation) => !await userValidationService.VerifyDniExistsAsync(dni)).WithMessage("El {PropertyName} ya existe, debe usar otro.")
        .MaximumLength(10).WithMessage("El campo {PropertyName} puede tener un maximo de 10 caracteres")
        .Matches(@"^\d+$").WithMessage("El campo {PropertyName} solo puede contener numeros");

        RuleFor(u => u.PhoneNumber)
            .NotEmpty().WithMessage("El campo {PropertyName} es obligatorio")
            .MustAsync(async(phoneNumber, cancellation) => !await userValidationService.VerifyPhoneNumberExistsAsync(phoneNumber)).WithMessage("El {PropertyName} ya se encuentra registrado, debe usar otro.")
            .MaximumLength(14).WithMessage("El campo {PropertyName} puede tener un maximo de 14 caracteres")
            .Matches(@"^\d+$").WithMessage("El campo {PropertyName} solo puede contener numeros");

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("El campo {PropertyName} es obligatorio")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El campo {PropertyName} solo puede contener letras")
            .MinimumLength(2).MaximumLength(200).WithMessage("El campo {PropertyName} debe tener un minimo de 2 caracteres");
    }
}

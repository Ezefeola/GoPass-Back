using FluentValidation;
using GoPass.Application.Services.Validations.Interfaces;
using GoPass.Domain.DTOs.Request.UserRequestDTOs;

namespace GoPass.Application.Validators.Users
{
    public class VerifySmsByEmailCodeValidator : AbstractValidator<VerifyphoneByEmailCodeRequestDto>
    {

        public VerifySmsByEmailCodeValidator(IUserValidationService userValidationService)
        {
            RuleFor(x => x.EmailCode)
            .NotEmpty()
                .WithMessage("El código es obligatorio.")
            .Must((code, cancellationToken) => userValidationService.VerifyEnteredCodeAsync(code.EmailCode))
                .WithMessage("El código ingresado no coincide con el que se le envió por SMS.");
        }
    }
}

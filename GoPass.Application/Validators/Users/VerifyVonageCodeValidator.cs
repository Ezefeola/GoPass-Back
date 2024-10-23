//using FluentValidation;
//using GoPass.Application.Services.Validations.Interfaces;
//using GoPass.Domain.DTOs.Request.UserRequestDTOs;

//namespace GoPass.Application.Validators.Users
//{
//    public class VerifyVonageCodeValidator : AbstractValidator<VerifyVonageCodeRequestDto>
//    {

//        public VerifyVonageCodeValidator(IUserValidationService userValidationService)
//        {
//            RuleFor(x => x.VonageCode)
//            .NotEmpty().WithMessage("El código es obligatorio.")
//                .MustAsync(async (code, cancellationToken) => await userValidationService.VerifyEnteredCodeAsync(code)).WithMessage("El código ingresado no coincide con el que se le envió por SMS.");
//        }
//    }
//}

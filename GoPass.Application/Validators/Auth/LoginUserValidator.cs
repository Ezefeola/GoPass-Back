using FluentValidation;
using GoPass.Domain.DTOs.Request.AuthRequestDTOs;

namespace GoPass.Application.Validators.Auth;

public class LoginUserValidator : AbstractValidator<LoginRequestDto>
{
    public LoginUserValidator()
    {
        RuleFor(u => u.Email).NotEmpty().WithMessage("El campo {PropertyName} No puede ser vacio ya que debe coincidir con las credenciales para loguearse");

        RuleFor(u => u.Password).NotEmpty().WithMessage("El campo {PropertyName} No puede ser vacio ya que debe coincidir con las credenciales para loguearse");
    }
}

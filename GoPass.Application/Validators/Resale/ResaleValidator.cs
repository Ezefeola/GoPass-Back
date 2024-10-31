using FluentValidation;
using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.ResaleRequestDTOs;

namespace GoPass.Application.Validators.Resale;

public class ResaleValidator : AbstractValidator<PublishResaleRequestDto>
{
    public ResaleValidator(ITicketService entradaService)
    {
        RuleFor(r => r.QrCode)
            .NotEmpty().WithMessage("El campo CodigoQR es obligatorio")
            .MustAsync(async (qrCode, _) => !await entradaService.VerifyQrCodeAsync(qrCode)).WithMessage("El {PropertyName} ya se encuentra registrado en nuestra base de datos");
    }
}

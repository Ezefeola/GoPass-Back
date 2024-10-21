using FluentValidation;
using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;

namespace GoPass.Application.Validators.Resale;

public class ResaleValidator : AbstractValidator<PublishReventaRequestDto>
{
    public ResaleValidator(IEntradaService entradaService)
    {
        RuleFor(r => r.CodigoQR)
            .NotEmpty().WithMessage("El campo CodigoQR es obligatorio")
            .MustAsync(async (qrCode, _) => !await entradaService.VerifyQrCodeAsync(qrCode)).WithMessage("El {PropertyName} ya se encuentra registrado en nuestra base de datos");
    }
}

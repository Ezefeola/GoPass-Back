using GoPass.Application.Services.Interfaces;

namespace GoPass.Application.ServiceFacade;

public interface IServiceFacade
{
    public IVonageSmsService vonageSmsService { get; }
    public IUsuarioService usuarioService { get; }
    public ITicketMasterService ticketMasterService { get; }
    public IEmailService emailService { get; }
    public IAesGcmCryptoService aesGcmCryptoService { get; }
    public IEntradaService entradaService { get; }
    public IGopassHttpClientService gopassHttpClientService { get; }
    public ITemplateService templateService { get; }
    public IReventaService reventaService { get; }
    public ITokenService tokenService { get; }
}

using GoPass.Application.Services.Interfaces;

namespace GoPass.Application.Facades.ServiceFacade;

public interface IServiceFacade
{
    public IVonageSmsService VonageSmsService { get; }
    public IUsuarioService UsuarioService { get; }
    public ITicketMasterService TicketMasterService { get; }
    public IEmailService EmailService { get; }
    public IAesGcmCryptoService AesGcmCryptoService { get; }
    public IEntradaService EntradaService { get; }
    public IGopassHttpClientService GopassHttpClientService { get; }
    public ITemplateService TemplateService { get; }
    public IReventaService ReventaService { get; }
    public ITokenService TokenService { get; }
    public IAuthService AuthService { get; }
    public IResaleTicketTransactionService ResaleTicketTransactionService { get; }
}

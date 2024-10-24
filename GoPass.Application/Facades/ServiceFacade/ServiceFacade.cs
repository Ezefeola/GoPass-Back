using GoPass.Application.Services.Interfaces;

namespace GoPass.Application.Facades.ServiceFacade;

public class ServiceFacade : IServiceFacade
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

    public ServiceFacade(IVonageSmsService vonageSmsService,
        IUsuarioService usuarioService,
        ITicketMasterService ticketMasterService,
        IEmailService emailService,
        IAesGcmCryptoService aesGcmCryptoService,
        IEntradaService entradaService,
        IGopassHttpClientService gopassHttpClientService,
        ITemplateService templateService,
        IReventaService reventaService,
        ITokenService tokenService,
        IAuthService authService,
        IResaleTicketTransactionService resaleTicketTransactionService
        )
    {
        VonageSmsService = vonageSmsService;
        UsuarioService = usuarioService;
        TicketMasterService = ticketMasterService;
        EmailService = emailService;
        AesGcmCryptoService = aesGcmCryptoService;
        EntradaService = entradaService;
        GopassHttpClientService = gopassHttpClientService;
        TemplateService = templateService;
        ReventaService = reventaService;
        TokenService = tokenService;
        AuthService = authService;
        ResaleTicketTransactionService = resaleTicketTransactionService;
    }
}

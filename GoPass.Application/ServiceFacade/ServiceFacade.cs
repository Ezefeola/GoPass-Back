using GoPass.Application.Services.Interfaces;

namespace GoPass.Application.ServiceFacade;

public class ServiceFacade : IServiceFacade
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

    public ServiceFacade(IVonageSmsService vonageSmsService,
        IUsuarioService usuarioService,
        ITicketMasterService ticketMasterService,
        IEmailService emailService,
        IAesGcmCryptoService aesGcmCryptoService,
        IEntradaService entradaService,
        IGopassHttpClientService gopassHttpClientService,
        ITemplateService templateService,
        IReventaService reventaService,
        ITokenService tokenService)
    {
        this.vonageSmsService = vonageSmsService;
        this.usuarioService = usuarioService;
        this.ticketMasterService = ticketMasterService;
        this.emailService = emailService;
        this.aesGcmCryptoService = aesGcmCryptoService;
        this.entradaService = entradaService;
        this.gopassHttpClientService = gopassHttpClientService;
        this.templateService = templateService;
        this.reventaService = reventaService;
        this.tokenService = tokenService;
    }
}

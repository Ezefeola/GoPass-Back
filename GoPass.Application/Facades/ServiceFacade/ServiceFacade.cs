using GoPass.Application.Services.Interfaces;

namespace GoPass.Application.Facades.ServiceFacade;

public class ServiceFacade : IServiceFacade
{
    public IUserService UserService { get; }
    public IEmailService EmailService { get; }
    public IAesGcmCryptoService AesGcmCryptoService { get; }
    public ITicketService TicketService { get; }
    public IGopassHttpClientService GopassHttpClientService { get; }
    public ITemplateService TemplateService { get; }
    public IResaleService ResaleService { get; }
    public ITokenService TokenService { get; }
    public IAuthService AuthService { get; }
    public IResaleTicketTransactionService ResaleTicketTransactionService { get; }
    public INotificationService NotificationService { get; }

    public ServiceFacade
        (
            IUserService userService,
            IEmailService emailService,
            IAesGcmCryptoService aesGcmCryptoService,
            ITicketService ticketService,
            IGopassHttpClientService gopassHttpClientService,
            ITemplateService templateService,
            IResaleService resaleService,
            ITokenService tokenService,
            IAuthService authService,
            IResaleTicketTransactionService resaleTicketTransactionService,
            INotificationService notificationService
        )
    {
        UserService = userService;
        EmailService = emailService;
        AesGcmCryptoService = aesGcmCryptoService;
        TicketService = ticketService;
        GopassHttpClientService = gopassHttpClientService;
        TemplateService = templateService;
        ResaleService = resaleService;
        TokenService = tokenService;
        AuthService = authService;
        ResaleTicketTransactionService = resaleTicketTransactionService;
        NotificationService = notificationService;
    }
}

using GoPass.Application.Services.Interfaces;

namespace GoPass.Application.Facades.ServiceFacade;

public interface IServiceFacade
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
}

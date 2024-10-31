using GoPass.Application.Notifications.Classes;
using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.NotificationDTOs;
using GoPass.Domain.DTOs.Response.TicketResaleHistoryDTOs;
using GoPass.Domain.Models;

public class NotificationService : INotificationService
{
    private readonly EmailNotificationBackgroundService _backgroundService;
    private readonly IUserService _usuarioService;

    public NotificationService(EmailNotificationBackgroundService backgroundService, IUserService usuarioService)
    {
        _backgroundService = backgroundService;
        _usuarioService = usuarioService;
    }

    public async Task NotifyBuyerAndSellerAsync(TicketResaleHistoryResponseDto ticketResaleHistoryResponseDto, CancellationToken cancellationToken)
    {
        User buyer = await _usuarioService.GetByIdAsync(ticketResaleHistoryResponseDto.BuyerId);
        User seller = await _usuarioService.GetByIdAsync(ticketResaleHistoryResponseDto.SellerId);

        SendNotificationAsync(buyer.Name!, buyer.Email, ticketResaleHistoryResponseDto.QRCode, "Compra realizada",
        $"¡Felicidades! ha comprado la entrada para el partido: {ticketResaleHistoryResponseDto.GameName}. Su código QR es: {ticketResaleHistoryResponseDto.QRCode}", cancellationToken);

        SendNotificationAsync(seller.Name!, seller.Email, ticketResaleHistoryResponseDto.QRCode, "Venta realizada",
        $"¡Felicidades! ha vendido la entrada para el partido: {ticketResaleHistoryResponseDto.GameName}. El código QR era: {ticketResaleHistoryResponseDto.QRCode}", cancellationToken);
    }

    private void SendNotificationAsync(string userName, string toEmail, string qrCode, string subject, string message, CancellationToken cancellationToken)
    {
        var notificationDto = new NotificationEmailRequestDto
        {
            UserName = userName,
            To = toEmail,
            TicketQrCode = qrCode,
            Subject = subject,
            Message = message
        };

        Subject<NotificationEmailRequestDto> notifier = new();
        var emailObserver = new EmailNotificationObserver(_backgroundService);
        notifier.Attach(emailObserver);

        notifier.Notify(notificationDto); 

        notifier.Detach(emailObserver); 
    }
}
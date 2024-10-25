using GoPass.Application.Notifications.Classes;
using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.NotificationDTOs;
using GoPass.Domain.Models;
using System.Diagnostics;

namespace GoPass.Application.Services.Classes
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly IUsuarioService _usuarioService;

        public NotificationService(IEmailService emailService, 
                IUsuarioService usuarioService
            )
        {
            _emailService = emailService;
            _usuarioService = usuarioService;
        }

        public async Task NotifyBuyerAndSellerAsync(HistorialCompraVenta historialCompraVenta, Entrada ticket, CancellationToken cancellationToken)
        {
            Usuario buyer = await _usuarioService.GetByIdAsync(historialCompraVenta.CompradorId);
            Usuario seller = await _usuarioService.GetByIdAsync(historialCompraVenta.VendedorId);

            var stopwatch = Stopwatch.StartNew();
            await SendNotificationAsync(buyer.Nombre!, buyer.Email, ticket.CodigoQR, "Compra realizada", cancellationToken);
            await SendNotificationAsync(seller.Nombre!, seller.Email, ticket.CodigoQR, "Venta realizada", cancellationToken);
            stopwatch.Stop();
            Console.WriteLine("El envio de la notificacion tomo:" + stopwatch.ElapsedMilliseconds);
        }

        private async Task SendNotificationAsync(string userName, string toEmail, string qrCode, string subject, CancellationToken cancellationToken)
        {
            var notificationDto = new NotificationEmailRequestDto
            {
                UserName = userName,
                To = toEmail,
                TicketQrCode = qrCode,
                Subject = subject
            };

            Subject<NotificationEmailRequestDto> notifier = new();
            var emailObserver = new EmailNotificationObserver(_emailService);
            notifier.Attach(emailObserver);
            await notifier.Notify(notificationDto);
            notifier.Detach(emailObserver);
        }
    }
}

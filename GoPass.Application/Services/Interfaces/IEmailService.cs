using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Request.NotificationDTOs;

namespace GoPass.Application.Services.Interfaces;

public interface IEmailService
{
   Task<bool> SendVerificationEmailAsync(EmailValidationRequestDto emailValidationRequestDto);
   Task<bool> SendNotificationEmailAsync(NotificationEmailRequestDto notificationEmailRequestDto);
   Task<bool> SetEmailParametersAsync(string templateName, string subject, Dictionary<string, string> valoresReemplazo, string recipientEmail);
}

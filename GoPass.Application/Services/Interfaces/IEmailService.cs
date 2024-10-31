using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Request.NotificationDTOs;

namespace GoPass.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendAccountVerificationEmailAsync(EmailValidationRequestDto emailValidationRequestDto);
        Task<bool> SendNotificationEmailAsync(NotificationEmailRequestDto notificationEmailRequestDto);
        Task<bool> SendSmsVerificationCodeEmailAsync(string email);
        Task<bool> SetEmailParametersAsync(string templateName, string subject, Dictionary<string, string> valoresReemplazo, string recipientEmail);
        bool VerifyCode(int userInputCode);
    }
}
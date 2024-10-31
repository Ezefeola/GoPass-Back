using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.NotificationDTOs;
using GoPass.Application.Utilities.Mappers;
using GoPass.Infrastructure.UnitOfWork;
using GoPass.Domain.Models;

namespace GoPass.Application.Services.Classes;

public class EmailService : IEmailService
{
    private static string _Host = "smtp.gmail.com";
    private static int _Port = 587;

    private static string _From = "Autenticacion";
    private static string _Email = "automatizaciones.sas@gmail.com";
    private static string _Password = "nnkyigaljcvbydhi";
    private readonly ITemplateService _templateService;
    private int _verificationCode;

    public EmailService
        (
            ITemplateService templateService
        )
    {
        _templateService = templateService;
    }

    public async Task<bool> SendSmsVerificationCodeEmailAsync(string email)
    {

        _verificationCode = new Random().Next(100000, 999999); 

        string subject = "Tu código de verificación";
        var valoresReemplazo = new Dictionary<string, string> { { "Code", _verificationCode.ToString() } };

        return await SetEmailParametersAsync("VerifySms", subject, valoresReemplazo, email);
    }

    public async Task<bool> SetEmailParametersAsync(string templateName, string subject, Dictionary<string, string> valoresReemplazo, string recipientEmail)
    {
        string contenidoPlantilla = await _templateService.ObtenerContenidoTemplateAsync(templateName, valoresReemplazo);
        EmailValidationRequestDto emailConfig = new();
        EmailValidationRequestDto emailToSend = emailConfig.AssignEmailValues(recipientEmail, subject, contenidoPlantilla);
        return await SendAccountVerificationEmailAsync(emailToSend);
    }
    public async Task<bool> SendAccountVerificationEmailAsync(EmailValidationRequestDto emailValidationRequestDto)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_From, _Email));
            email.To.Add(MailboxAddress.Parse(emailValidationRequestDto.To));
            email.Subject = emailValidationRequestDto.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = emailValidationRequestDto.Body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_Host, _Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_Email, _Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> SendNotificationEmailAsync(NotificationEmailRequestDto notificationEmailRequestDto)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_From, _Email));
            email.To.Add(MailboxAddress.Parse(notificationEmailRequestDto.To));
            email.Subject = notificationEmailRequestDto.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = notificationEmailRequestDto.Message };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_Host, _Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_Email, _Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            return false;
        }
    }
    public bool VerifyCode(int userInputCode)
    {
        if (userInputCode == _verificationCode)
        {
            Console.WriteLine("Código verificado con éxito ✅");
            return true;
        }
        return false;
    }
}

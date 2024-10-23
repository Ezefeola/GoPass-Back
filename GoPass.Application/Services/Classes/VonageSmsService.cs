using GoPass.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;

namespace GoPass.Application.Services.Classes;

public class VonageSmsService : IVonageSmsService
{
    private readonly IConfiguration _configuration;
    private int _verificationCode;
    public VonageSmsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendVonageVerificationCode(string phoneNumber)
    {
        var apiKey = _configuration["Vonage:VonageApiKey"];
        var apiSecret = _configuration["Vonage:ApiSecret"];

        var credentials = Credentials.FromApiKeyAndSecret(apiKey, apiSecret);

        var client = new VonageClient(credentials);

        _verificationCode = new Random().Next(100000, 999999);

        try
        {
            
            var response = await client.SmsClient.SendAnSmsAsync(new SendSmsRequest
            {
                To = phoneNumber, 
                From = "GopassTest", 
                Text = $"Tu código de verificación es {_verificationCode}" 
            });

            var message = response.Messages.FirstOrDefault(); 

            
            if (message != null && message.Status == "0")
            {
                Console.WriteLine("SMS enviado con éxito 🚀");
                return true;
            }
            else
            {
                Console.WriteLine($"Error: {message?.ErrorText ?? "No se pudo enviar el SMS"}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar código: {ex.Message}");
            return false;

        }
    }

    public async Task<bool> VerifyCode(int userInputCode)
    {
        if (userInputCode == _verificationCode)
        {
            Console.WriteLine("Código verificado con éxito ✅");
            return true;
        }
        else
        {
            Console.WriteLine("Código incorrecto ❌");
            return false;
        }
    }
}

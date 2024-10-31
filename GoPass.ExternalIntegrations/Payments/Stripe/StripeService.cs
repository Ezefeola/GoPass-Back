using Microsoft.Extensions.Configuration;
using Stripe;

namespace GoPass.ExternalIntegrations.Payments.Stripe;

public class StripeService
{
    private readonly IConfiguration _configuration;

    public StripeService(IConfiguration configuration)
    {
        _configuration = configuration;
        StripeConfiguration.ApiKey = _configuration["Stripe:ApiKey"];
    }

    public async Task<string> CreateAndConfirmPaymentIntentAsync(decimal amount, string paymentMethodId, string currency = "usd")
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Convertir a centavos
                Currency = currency,
                PaymentMethod = paymentMethodId,
                ConfirmationMethod = "manual", // Permite confirmar manualmente el pago
                Confirm = true // Confirmar automáticamente el PaymentIntent
            };
            var paymentIntent = await paymentIntentService.CreateAsync(options);

            // Retorna el PaymentIntentId solo si el pago fue exitoso
            return paymentIntent.Status == "succeeded" ? paymentIntent.Id : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al procesar el pago: {ex.Message}");
            return null;
        }
    }

    public async Task<string> CreatePaymentIntentAsync(decimal amount, string currency = "usd")
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" }
            };
            var paymentIntent = await paymentIntentService.CreateAsync(options);

            return paymentIntent.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear PaymentIntent: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> ConfirmPaymentIntentAsync(string paymentIntentId, string paymentMethodId)
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentConfirmOptions
            {
                PaymentMethod = paymentMethodId
            };
            var paymentIntent = await paymentIntentService.ConfirmAsync(paymentIntentId, options);

            return paymentIntent.Status == "succeeded";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al confirmar PaymentIntent: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> VerifyPaymentAsync(string paymentIntentId)
    {
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);
        return paymentIntent.Status == "succeeded";
    }


}

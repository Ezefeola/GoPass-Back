using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.PaymentRequestDTOs;
using MercadoPago.Client.CardToken;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.CardToken;
using MercadoPago.Resource.Payment;

namespace GoPass.ExternalIntegrations.Payments;

public class MercadoPagoService 
{
    public MercadoPagoService()
    {
        MercadoPagoConfig.AccessToken = "TEST-4159010736371694-102919-90a0afa3ce3461fea3b70e6af8270021-1063419036"; // cargar desde la configuración
    }

    public async Task<PaymentStatusDto> CreatePaymentAsync(PaymentRequestDto paymentRequestDto)
    {

        var payment = new PaymentCreateRequest
        {
            TransactionAmount = paymentRequestDto.Amount,
            Description = paymentRequestDto.Description,
            PaymentMethodId = paymentRequestDto.PaymentMethodId,
            PaymentMethodOptionId = paymentRequestDto.PaymentMethodId,
            Token = "TEST-4159010736371694-102919-90a0afa3ce3461fea3b70e6af8270021-1063419036",
            Payer = new PaymentPayerRequest
            {
                Email = "test@gmail.com",

            }

        };

        var client = new PaymentClient();
        Payment result = await client.CreateAsync(payment);

        return new PaymentStatusDto
        {
            Status = result.Status
        };
    }

    public async Task<PaymentStatusDto> VerifyPaymentStatusAsync(string paymentId)
    {
        // Lógica para consultar el estado del pago con MercadoPago
        PaymentStatusDto paymentTest = new PaymentStatusDto()
        {
            Status = "approved",
            Amount = 100
        };
        return paymentTest;
    }

    public async Task<bool> ProcessRefundAsync(string paymentId)
    {
        // Lógica para procesar un reembolso con MercadoPago
        return false;
    }

 }

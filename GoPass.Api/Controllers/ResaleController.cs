using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.Models;
using GoPass.Application.Facades.ServiceFacade;
using GoPass.Domain.DTOs.Response.UserResponseDTOs;
using GoPass.Domain.DTOs.Request.ResaleRequestDTOs;
using GoPass.Domain.DTOs.Response.ResaleResponseDTOs;
using GoPass.ExternalIntegrations.Payments.Stripe;
using System.Threading;
using GoPass.Domain.DTOs.Request.PaymentRequestDTOs.Stripe;
using GoPass.Domain.DTOs.Response.TicketResaleHistoryDTOs;

namespace GoPass.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResaleController : ControllerBase
{
    private readonly IServiceFacade _serviceFacade;
    private readonly StripeService _stripeService;

    public ResaleController
        (
            IServiceFacade serviceFacade,
            StripeService stripeService
        )
    {
        _serviceFacade = serviceFacade;
        _stripeService = stripeService;
    }

    [Authorize]
    [HttpGet("get-resales")]
    public async Task<IActionResult> GetResales([FromQuery] PaginationDto paginationDto)
    {
        List<Resale> resales = await _serviceFacade.ResaleService.GetAllWithPaginationAsync(paginationDto);

        return Ok(resales);
    }

    [Authorize]
    [HttpGet("get-seller-information")]
    public async Task<IActionResult> GetTicketResaleSellerInformation(int vendedorId)
    {
        User sellerInformation = await _serviceFacade.UserService.GetByIdAsync(vendedorId);

        SellerInformationResponseDto sellerInformationResponseDto = sellerInformation.MapToSellerInfoResponseDto();

        return Ok(sellerInformationResponseDto);
    }

    [Authorize]
    [HttpPost("publish-resale-ticket")]
    public async Task<IActionResult> PublishResaleTicket(PublishResaleRequestDto publishResaleRequestDto, CancellationToken cancellationToken)
    {
        try
        {
            int userId = _serviceFacade.AuthService.GetUserIdFromToken();

            bool validUserCredentials = await _serviceFacade.UserService.ValidateUserCredentialsToPublishTicket(userId);

            if (validUserCredentials == false) return BadRequest("Debe tener todas sus credenciales en regla para poder publicar una entrada");

            PublishTicketRequestDto verifiedTicket = await _serviceFacade.GopassHttpClientService.GetTicketByQrAsync(publishResaleRequestDto.QrCode);

            PublishResaleResponseDto publishResaleResponseDto = await _serviceFacade.ResaleTicketTransactionService.PublishResaleTicketAsync(verifiedTicket, publishResaleRequestDto, userId, cancellationToken);

            return Ok(publishResaleResponseDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Authorize]
    [HttpPut("buy-ticket")]
    public async Task<IActionResult> BuyTicket(BuyTicketRequestDto buyTicketRequestDto, CancellationToken cancellationToken)
    {
        int userId = _serviceFacade.AuthService.GetUserIdFromToken();

        TicketResaleHistoryResponseDto buyedTicketDataResponse = await _serviceFacade.ResaleService.BuyTicketAsync(buyTicketRequestDto.TicketId, userId, cancellationToken);

        await _serviceFacade.NotificationService.NotifyBuyerAndSellerAsync(buyedTicketDataResponse, cancellationToken);

        return Ok(buyedTicketDataResponse);
    }

    //[Authorize]
    //[HttpPut("buy-ticket")]
    //public async Task<IActionResult> BuyTicket(BuyTicketRequestDto buyTicketRequestDto, CancellationToken cancellationToken)
    //{
    //    int userId = _serviceFacade.AuthService.GetUserIdFromToken();

    //    // 1. Verificar que el ticket esté disponible para la reventa
    //    var ticketToBuy = await _serviceFacade.ResaleService.GetResaleByTicketIdAsync(buyTicketRequestDto.TicketId);
    //    if (ticketToBuy == null)
    //    {
    //        return NotFound("Ticket no encontrado o no disponible para la compra.");
    //    }

    //    // 2. Crear un PaymentIntent en Stripe y obtener el client secret
    //    decimal amountToCharge = ticketToBuy.Price; // Define el precio de la entrada
    //    var paymentIntentId = await _stripeService.CreateAndConfirmPaymentIntentAsync(amountToCharge);

    //    // 3. Validar si el pago fue exitoso
    //    if (string.IsNullOrEmpty(paymentIntentId))
    //    {
    //        return BadRequest("No se pudo confirmar el pago.");
    //    }

    //    // 4. Confirmar la compra del ticket
    //    var boughtTicketDataResponse = await _serviceFacade.ResaleService.BuyTicketAsync(buyTicketRequestDto.TicketId, userId, cancellationToken);

    //    // 5. Notificar al comprador y al vendedor
    //    await _serviceFacade.NotificationService.NotifyBuyerAndSellerAsync(boughtTicketDataResponse, cancellationToken);

    //    return Ok(boughtTicketDataResponse);
    //}

    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] int ticketId)
    {
        var ticketToBuyData = await _serviceFacade.ResaleService.GetResaleByTicketIdAsync(ticketId);
        var paymentIntentId = await _stripeService.CreatePaymentIntentAsync(ticketToBuyData.Price, "usd");

        if (string.IsNullOrEmpty(paymentIntentId))
        {
            return BadRequest("Error al crear PaymentIntent");
        }

        return Ok(new { PaymentIntentId = paymentIntentId });
    }

    // Endpoint para confirmar el PaymentIntent
    [HttpPost("confirm-payment-intent")]
    public async Task<IActionResult> ConfirmPaymentIntent([FromBody] ConfirmPaymentIntentRequestDto confirmRequest, CancellationToken cancellationToken)
    {
        var isConfirmed = await _stripeService.ConfirmPaymentIntentAsync(confirmRequest.PaymentIntentId, confirmRequest.PaymentMethodId);

        if (!isConfirmed)
        {
            return BadRequest("Error al confirmar PaymentIntent");
        }

        // Aquí puedes continuar con la compra del ticket si el pago fue exitoso
        var buyTicketResponse = await _serviceFacade.ResaleService.BuyTicketAsync(confirmRequest.TicketId, confirmRequest.UserId, cancellationToken);

        await _serviceFacade.NotificationService.NotifyBuyerAndSellerAsync(buyTicketResponse, cancellationToken);

        return Ok(buyTicketResponse);
    }

}

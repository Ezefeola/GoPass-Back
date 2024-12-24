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

}

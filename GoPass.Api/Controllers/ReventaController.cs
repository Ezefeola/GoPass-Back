using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.Models;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.DTOs.Request.NotificationDTOs;
using GoPass.Application.Notifications.Classes;
using GoPass.Application.Facades.ServiceFacade;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System.Diagnostics;
using GoPass.Domain.DTOs.Response;

namespace GoPass.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReventaController : ControllerBase
{
    private readonly IServiceFacade _serviceFacade;
    private readonly ICustomAutoMapper _customAutoMapper;

    public ReventaController(IServiceFacade serviceFacade, ICustomAutoMapper customAutoMapper)
    {
        _serviceFacade = serviceFacade;
        _customAutoMapper = customAutoMapper;
    }

    [Authorize]
    [HttpGet("get-resales")]
    public async Task<IActionResult> GetResales([FromQuery] PaginationDto paginationDto)
    {
        List<Reventa> resales = await _serviceFacade.ReventaService.GetAllWithPaginationAsync(paginationDto);

        return Ok(resales);
    }

    [Authorize]
    [HttpGet("get-seller-information")]
    public async Task<IActionResult> GetTicketResaleSellerInformation(int vendedorId)
    {
        Usuario sellerInformation = await _serviceFacade.UsuarioService.GetByIdAsync(vendedorId);

        SellerInformationResponseDto sellerInformationResponseDto = sellerInformation.MapToSellerInfoResponseDto();

        return Ok(sellerInformationResponseDto);
    }

    [HttpGet("get-ticket-from-faker")]
    public async Task<IActionResult> GetTicketFromTicketFaker(string codigoQr)
    {
        Entrada verifiedTicket = await _serviceFacade.GopassHttpClientService.GetTicketByQrAsync(codigoQr);
        var entradaResponse = _customAutoMapper.Map<Entrada, EntradaResponseDto>(verifiedTicket!);
        return Ok(entradaResponse);
    }

    [HttpGet("validate-ticket-from-faker")]
    public async Task<IActionResult> ValidateTicketFromTicketFaker(string codigoQr)
    {
        Entrada verifiedTicket = await _serviceFacade.GopassHttpClientService.GetTicketByQrAsync(codigoQr);

        if (verifiedTicket is null) return BadRequest("No se encontro la entrada a validar.");

        verifiedTicket.Verificada = true;

        return Ok(verifiedTicket);
    }

    [Authorize]
    [HttpPost("publicar-entrada-reventa")]
    public async Task<IActionResult> PublishResaleTicket(PublishReventaRequestDto publishReventaRequestDto, CancellationToken cancellationToken)
    {
        try
        {
            int userId = await _serviceFacade.AuthService.GetUserIdFromTokenAsync();

            bool validUserCredentials = await _serviceFacade.UsuarioService.ValidateUserCredentialsToPublishTicket(userId);

            if (validUserCredentials == false) return BadRequest("Debe tener todas sus credenciales en regla para poder publicar una entrada");

            Entrada verifiedTicket = await _serviceFacade.GopassHttpClientService.GetTicketByQrAsync(publishReventaRequestDto.CodigoQR);

            var stopwatch = Stopwatch.StartNew();
            Reventa reventaToPublish = _customAutoMapper.Map<PublishReventaRequestDto, Reventa>(publishReventaRequestDto);
            stopwatch.Stop();
            Console.WriteLine("El mapeo de publishReventaRequestDto a Reventa en ReventaController tomo:" + stopwatch.ElapsedMilliseconds);
            Reventa publishedReventa = await _serviceFacade.ResaleTicketTransactionService.PublishResaleTicketAsync(verifiedTicket, reventaToPublish, userId, cancellationToken);

            return Ok(publishedReventa.MapToResponseDto());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("comprar-entrada")]
    public async Task<IActionResult> BuyTicket(BuyEntradaRequestDto buyEntradaRequestDto, CancellationToken cancellationToken)
    {
        int userId = await _serviceFacade.AuthService.GetUserIdFromTokenAsync();

        Reventa resaleDb = await _serviceFacade.ReventaService.GetResaleByEntradaIdAsync(buyEntradaRequestDto.EntradaId);

        if(userId == resaleDb.VendedorId)
        {
            return BadRequest("Esta intentando comprar su propia entrada, lo cual no tiene sentido");
        }

        Entrada ticketDb = await _serviceFacade.EntradaService.GetByIdAsync(buyEntradaRequestDto.EntradaId);
        HistorialCompraVenta publishReventaBuyer = await _serviceFacade.ReventaService.BuyTicketAsync(resaleDb.Id, userId, cancellationToken);

        Usuario buyerData = await _serviceFacade.UsuarioService.GetByIdAsync(publishReventaBuyer.CompradorId);
        Usuario sellerData = await _serviceFacade.UsuarioService.GetByIdAsync(publishReventaBuyer.VendedorId);
       

        Subject<NotificationEmailRequestDto> purchaseNotifier = new();
        BuyerEmailNotificationObserver compradorObserver = new BuyerEmailNotificationObserver(_serviceFacade.EmailService);

        purchaseNotifier.Attach(compradorObserver);

        NotificationEmailRequestDto buyerNotificationEmailRequestDto = new NotificationEmailRequestDto
        {
            UserName = buyerData.Nombre!,
            To = buyerData.Email,
            TicketQrCode = ticketDb.CodigoQR

        };
        await purchaseNotifier.Notify(buyerNotificationEmailRequestDto); // Comprador

        Subject<NotificationEmailRequestDto> sellerNotifier = new();
        SellerEmailNotificationObserver sellerObserver = new SellerEmailNotificationObserver(_serviceFacade.EmailService);

        sellerNotifier.Attach(sellerObserver);

        NotificationEmailRequestDto sellerNotificationEmailRequestDto = new NotificationEmailRequestDto
        {
            UserName = sellerData.Nombre!,
            To = sellerData.Email,
            TicketQrCode = ticketDb.CodigoQR

        };
        await sellerNotifier.Notify(sellerNotificationEmailRequestDto); // Vendedor

        return Ok(publishReventaBuyer);
    }
}

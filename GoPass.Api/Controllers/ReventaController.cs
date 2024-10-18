using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.Models;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.DTOs.Request.NotificationDTOs;
using GoPass.Application.Notifications.Classes;

namespace GoPass.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReventaController : ControllerBase
    {
        private readonly IReventaService _reventaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IEntradaService _entradaService;
        private readonly IGopassHttpClientService _gopassHttpClientService;
        private readonly IEmailService _emailService;

        public ReventaController(IReventaService reventaService, IUsuarioService usuarioService, IEntradaService entradaService,
            IGopassHttpClientService gopassHttpClientService, IEmailService emailService)
        {
            _reventaService = reventaService;
            _usuarioService = usuarioService;
            _entradaService = entradaService;
            _gopassHttpClientService = gopassHttpClientService;
            _emailService = emailService;
        }

        [Authorize]
        [HttpGet("get-resales")]
        public async Task<IActionResult> GetResales([FromQuery] PaginationDto paginationDto)
        {
            List<Reventa> resales = await _reventaService.GetAllWithPaginationAsync(paginationDto);

            return Ok(resales);
        }

        [Authorize]
        [HttpGet("get-seller-information")]
        public async Task<IActionResult> GetTicketResaleSellerInformation(int vendedorId)
        {
            Usuario sellerInformation = await _usuarioService.GetByIdAsync(vendedorId);

            SellerInformationResponseDto sellerInformationResponseDto = sellerInformation.FromModelToSellerInformationResponseDto();

            return Ok(sellerInformationResponseDto);
        }

        [HttpGet("get-ticket-from-faker")]
        public async Task<IActionResult> GetTicketFromTicketFaker(string codigoQr)
        {
            Entrada verifiedTicket = await _gopassHttpClientService.GetTicketByQrAsync(codigoQr);

            return Ok(verifiedTicket);
        }

        [HttpGet("validate-ticket-from-faker")]
        public async Task<IActionResult> ValidateTicketFromTicketFaker(string codigoQr)
        {
            Entrada verifiedTicket = await _gopassHttpClientService.GetTicketByQrAsync(codigoQr);

            if (verifiedTicket is null) return BadRequest("No se encontro la entrada a validar.");

            verifiedTicket.Verificada = true;

            return Ok(verifiedTicket);
        }

        [Authorize]
        [HttpPost("publicar-entrada-reventa")]
        public async Task<IActionResult> PublishResaleTicket(PublishReventaRequestDto publishReventaRequestDto)
        {
            try
            {
                int userId = await _usuarioService.GetUserIdFromTokenAsync();

                bool validUserCredentials = await _usuarioService.ValidateUserCredentialsToPublishTicket(userId);

                if (validUserCredentials == false) return BadRequest("Debe tener todas sus credenciales en regla para poder publicar una entrada");

                Entrada verifiedTicket = await _gopassHttpClientService.GetTicketByQrAsync(publishReventaRequestDto.CodigoQR);
                PublishEntradaRequestDto existingTicketInFaker = verifiedTicket.FromModelToPublishEntradaRequest();

                Entrada createdTicket = await _entradaService.PublishTicket(existingTicketInFaker, userId);

                Reventa reventaToPublish = publishReventaRequestDto.FromPublishReventaRequestToModel();
                reventaToPublish.EntradaId = createdTicket.Id;

                Reventa publishedReventa = await _reventaService.PublishTicketAsync(reventaToPublish, userId);

                return Ok(publishedReventa.FromModelToPublishReventaResponseDto());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("comprar-entrada")]
        public async Task<IActionResult> BuyTicket(BuyEntradaRequestDto buyEntradaRequestDto)
        {
            int userId = await _usuarioService.GetUserIdFromTokenAsync();

            Reventa resaleDb = await _reventaService.GetResaleByEntradaIdAsync(buyEntradaRequestDto.EntradaId);

            if(userId == resaleDb.VendedorId)
            {
                return BadRequest("Esta intentando comprar su propia entrada, lo cual no tiene sentido");
            }

            Entrada ticketDb = await _entradaService.GetByIdAsync(buyEntradaRequestDto.EntradaId);
            HistorialCompraVenta publishReventaBuyer = await _reventaService.BuyTicketAsync(resaleDb.Id, userId);

            Usuario buyerData = await _usuarioService.GetByIdAsync(publishReventaBuyer.CompradorId);
            Usuario sellerData = await _usuarioService.GetByIdAsync(publishReventaBuyer.VendedorId);
           

            Subject<NotificationEmailRequestDto> purchaseNotifier = new();
            BuyerEmailNotificationObserver compradorObserver = new BuyerEmailNotificationObserver(_emailService);

            purchaseNotifier.Attach(compradorObserver);

            NotificationEmailRequestDto buyerNotificationEmailRequestDto = new NotificationEmailRequestDto
            {
                UserName = buyerData.Nombre!,
                To = buyerData.Email,
                TicketQrCode = ticketDb.CodigoQR

            };
            await purchaseNotifier.Notify(buyerNotificationEmailRequestDto); // Comprador

            Subject<NotificationEmailRequestDto> sellerNotifier = new();
            SellerEmailNotificationObserver sellerObserver = new SellerEmailNotificationObserver(_emailService);

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
}

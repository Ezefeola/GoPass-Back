using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoPass.Domain.Models;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Application.Facades.ServiceFacade;
using GoPass.Domain.DTOs.Response.TicketFakerResponseDTOs;
using GoPass.Domain.DTOs.Response.TicketResponseDTOs;

namespace GoPass.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly IServiceFacade _serviceFacade;
    private readonly ILogger<TicketController>? _logger;

    public TicketController(IServiceFacade serviceFacade, ILogger<TicketController>? logger)
    {
        _serviceFacade = serviceFacade;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("get-tickets")]
    public async Task<IActionResult> GetTickets([FromQuery] PaginationDto paginationDto)
    {
        List<TicketResponseDto> tickets = await _serviceFacade.TicketService.GetAllTicketsWithPaginationAsync(paginationDto);

        return Ok(tickets);
    }

    [HttpGet("get-ticket-from-faker")]
    public async Task<IActionResult> GetTicketFromTicketFaker(string codigoQr)
    {
        TicketInFakerResponseDto verifiedTicket = await _serviceFacade.TicketService.GetTicketFromFakerByQr(codigoQr);

        return Ok(verifiedTicket);
    }

    [HttpGet("validate-ticket-from-faker")]
    public async Task<IActionResult> ValidateTicketFromTicketFaker(string codigoQr)
    {
        TicketInFakerResponseDto verifiedTicket = await _serviceFacade.TicketService.GetTicketFromFakerByQr(codigoQr);

        if (verifiedTicket is null) return BadRequest("No se encontro la entrada a validar.");


        return Ok(verifiedTicket);
    }
}
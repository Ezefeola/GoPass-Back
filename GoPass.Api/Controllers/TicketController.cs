using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoPass.Application.Utilities.Exceptions;
using GoPass.Application.Constants;
using GoPass.Domain.Models;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.DTOs.Response;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Application.ServiceFacade;

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
        List<Entrada> tickets = await _serviceFacade.EntradaService.GetAllWithPaginationAsync(paginationDto);

        return Ok(tickets);
    }

    [Authorize]
    [HttpPost(Endpoints.TICKET_VERIFY)]
    public async Task<IActionResult> VerificarEntrada([FromBody] VerifyEntradaRequestDto verifyEntradaRequestDto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Entrada ticket = await _serviceFacade.TicketMasterService.VerificarEntrada(verifyEntradaRequestDto.CodigoQR);

            EntradaResponseDto entradaResponse = ticket.MapToResponseDto();

            return Ok(entradaResponse);
        }
        catch (TicketVerificationException ex)
        {
            _logger!.LogError(ex.Message);
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger!.LogError(ex, Messages.ERR_TICKET_VERIFY);
            return StatusCode(StatusCodes.Status500InternalServerError, $"{Messages.ERR_TICKET_VERIFY} - {ex.Message}");
        }
    }
}

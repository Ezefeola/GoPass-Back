using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketFacker.Api.Data;
using TicketFacker.Api.Mappers;
using TicketFacker.Api.Models;

namespace TicketFacker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FakerController : ControllerBase
{
    private readonly ApplicationDbContext _applicationDbContext;

    public FakerController(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    [HttpGet("get-tickets-faker")]
    public async Task<IActionResult> Get()
    {
        List<Ticket> tickets = await _applicationDbContext.Tickets.AsNoTracking().ToListAsync();

        return Ok(tickets);
    }

    [HttpGet("get-by-qr/{codigoQr}")]
    public async Task<IActionResult> GetByTicketQr(string codigoQr)
    {
        bool CodeIsFound = await _applicationDbContext.Tickets.Where(x => x.QrCode == codigoQr).AnyAsync();

        if (CodeIsFound is false)
        {
            return NotFound("No se encontro la entrada con el codigo QR correspondiente.");
        }

        Ticket? ticket = await _applicationDbContext.Tickets.AsNoTracking().Where(x => x.QrCode == codigoQr).FirstOrDefaultAsync();

        return Ok(ticket!.ToResponseDto());
    }
}

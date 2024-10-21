using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketFaker.API.Data;
using TicketFaker.API.Models;

namespace TicketFaker.API.Controllers;

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
        List<Ticket> tickets = await _applicationDbContext.Tickets.ToListAsync();

        return Ok(tickets);
    }

    [HttpGet("get-by-qr/{codigoQr}")]
    public async Task<IActionResult> GetByTicketQr(string codigoQr)
    {
        bool CodeIsFound = await _applicationDbContext.Tickets.Where(x => x.CodigoQR == codigoQr).AnyAsync();

        if(CodeIsFound is false)
        {
            return NotFound("No se encontro la entrada con el codigo QR correspondiente.");
        }

        Ticket? ticket = await _applicationDbContext.Tickets.Where(x => x.CodigoQR == codigoQr).FirstOrDefaultAsync();

        return Ok(ticket);
    }
}

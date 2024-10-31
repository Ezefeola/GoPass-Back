using GoPass.Application.Facades.ServiceFacade;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Request.UserRequestDTOs;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.DTOs.Response.TicketResaleHistoryDTOs;
using GoPass.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoPass.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly ISmappfter _customAutoMapper;
    private readonly IServiceFacade _serviceFacade;

    public UserController(ILogger<UserController> logger, 
            ISmappfter customAutoMapper,
            IServiceFacade serviceFacade
        )
    {
        _logger = logger;
        _customAutoMapper = customAutoMapper;
        _serviceFacade = serviceFacade;
    }

    [Authorize]
    [HttpGet("user-credentials")]
    public async Task<IActionResult> GetUserCredentials()
    {
        int userId = _serviceFacade.AuthService.GetUserIdFromToken();
        User dbExistingUserCredentials = await _serviceFacade.UserService.GetByIdAsync(userId);

        dbExistingUserCredentials.DNI = _serviceFacade.AesGcmCryptoService.Decrypt(dbExistingUserCredentials.DNI!);
        dbExistingUserCredentials.PhoneNumber = _serviceFacade.AesGcmCryptoService.Decrypt(dbExistingUserCredentials.PhoneNumber!);

        return Ok(dbExistingUserCredentials);
    }

    [Authorize]
    [HttpPut("modify-user-credentials")]
    public async Task<IActionResult> ModifyUserCredentials(ModifyUsuarioRequestDto modifyUsuarioRequestDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            int userId = _serviceFacade.AuthService.GetUserIdFromToken();
            User dbExistingUserCredentials = await _serviceFacade.UserService.GetByIdAsync(userId);

            User credentialsToModify = _customAutoMapper.Map(modifyUsuarioRequestDto, dbExistingUserCredentials);

            credentialsToModify.IsVerified = true;
            User modifiedCredentials = await _serviceFacade.UserService.ModifyUserCredentialsAsync(userId, credentialsToModify, cancellationToken);

            return Ok(modifiedCredentials.MapToModifyUserDataResponseDto());

        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost("verify-phone")]
    public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
    {
        int userId = _serviceFacade.AuthService.GetUserIdFromToken();
         UserResponseDto userInDb = await _serviceFacade.UserService.GetUserByIdAsync(userId);
        var result = await _serviceFacade.EmailService.SendSmsVerificationCodeEmailAsync(userInDb.Email);

        if (result)
        {
            return Ok(new { message = "Código de verificación enviado exitosamente." });
        }

        return BadRequest(new { message = "Error al enviar el código de verificación." });
    }

    [HttpPost("verify-provided-code")]
    public async Task<IActionResult> VerifyPhoneByEmailCodeProvided(VerifyphoneByEmailCodeRequestDto verifyPhoneByEmailCodeRequestDto, CancellationToken cancellationToken)
    {
        int userId = _serviceFacade.AuthService.GetUserIdFromToken();
        User dbExistingUserCredentials = await _serviceFacade.UserService.GetByIdAsync(userId);

        dbExistingUserCredentials.IsSmsVerified = true;
        User modifiedCredentials = await _serviceFacade.UserService.UpdateAsync(userId, dbExistingUserCredentials, cancellationToken);

        return Ok("Se verifico su numero de telefono correctamente");
    }

    [HttpGet("obtener-usuario-entradas-reventa")]
    public async Task<IActionResult> GetUserResales()
    {
        try
        {
           int userId = _serviceFacade.AuthService.GetUserIdFromToken();

            List<Ticket> resales = await _serviceFacade.TicketService.GetTicketsInResaleByUserIdAsync(userId);

            return Ok(resales);
        }
        catch (Exception)
        {

            return BadRequest("No tenes entradas en reventa.");
        }
    }



    [HttpGet("obtener-usuario-entradas-compradas")]
    public async Task<IActionResult> GetUserTicketsBought()
    {
        try
        {
            int userId = _serviceFacade.AuthService.GetUserIdFromToken();

            List<TicketResaleHistoryResponseDto> ticketsBougthByUser = await _serviceFacade.ResaleService.GetBoughtTicketsByBuyerIdAsync(userId);

            return Ok(ticketsBougthByUser);
        }
        catch (Exception)
        {

            return BadRequest("No tenes entradas compradas.");
        }
    }
}
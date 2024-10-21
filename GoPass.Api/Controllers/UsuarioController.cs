using GoPass.Application.ServiceFacade;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoPass.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuarioController : ControllerBase
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IServiceFacade _serviceFacade;

    public UsuarioController(ILogger<UsuarioController> logger, 
            IServiceFacade serviceFacade
        )
    {
        _logger = logger;
        _serviceFacade = serviceFacade;
    }

    [Authorize]
    [HttpGet("user-credentials")]
    public async Task<IActionResult> GetUserCredentials()
    {
        int userId = await _serviceFacade.usuarioService.GetUserIdFromTokenAsync();
        Usuario dbExistingUserCredentials = await _serviceFacade.usuarioService.GetByIdAsync(userId);

        dbExistingUserCredentials.DNI = _serviceFacade.aesGcmCryptoService.Decrypt(dbExistingUserCredentials.DNI!);
        dbExistingUserCredentials.NumeroTelefono = _serviceFacade.aesGcmCryptoService.Decrypt(dbExistingUserCredentials.NumeroTelefono!);

        return Ok(dbExistingUserCredentials);
    }

    [Authorize]
    [HttpPut("modify-user-credentials")]
    public async Task<IActionResult> ModifyUserCredentials(ModifyUsuarioRequestDto modifyUsuarioRequestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            int userId = await _serviceFacade.usuarioService.GetUserIdFromTokenAsync();
            Usuario dbExistingUserCredentials = await _serviceFacade.usuarioService.GetByIdAsync(userId);

            if (await _serviceFacade.usuarioService.VerifyDniExistsAsync(modifyUsuarioRequestDto.DNI, userId))
            {
                return BadRequest("El DNI ya se encuentra registrado por otro usuario.");
            }

            if (await _serviceFacade.usuarioService.VerifyPhoneNumberExistsAsync(modifyUsuarioRequestDto.NumeroTelefono, userId))
            {
                return BadRequest("El número de teléfono ya se encuentra registrado por otro usuario.");
            }

            Usuario credentialsToModify = modifyUsuarioRequestDto.FromModifyUsuarioRequestToModel(dbExistingUserCredentials);

            credentialsToModify.DNI = _serviceFacade.aesGcmCryptoService.Encrypt(credentialsToModify.DNI!);
            credentialsToModify.NumeroTelefono = _serviceFacade.aesGcmCryptoService.Encrypt(credentialsToModify.NumeroTelefono!);

            if(credentialsToModify.DNI is not null && credentialsToModify.Nombre is not null && credentialsToModify.NumeroTelefono is not null)
            {
                credentialsToModify.Verificado = true;
            }

            Usuario modifiedCredentials = await _serviceFacade.usuarioService.Update(userId, credentialsToModify);

            modifiedCredentials.DNI = _serviceFacade.aesGcmCryptoService.Decrypt(credentialsToModify.DNI!);
            modifiedCredentials.NumeroTelefono = _serviceFacade.aesGcmCryptoService.Decrypt(credentialsToModify.NumeroTelefono!);
            return Ok(modifiedCredentials);

        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost("verify-phone")]
    public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
    {
        var result = await _serviceFacade.vonageSmsService.SendVonageVerificationCode(phoneNumber);

        if (result)
        {
            return Ok(new { message = "Código de verificación enviado exitosamente." });
        }

        return BadRequest(new { message = "Error al enviar el código de verificación." });
    }

    [HttpPost("verify-provided-code")]
    public async Task<IActionResult> VerifyVonageCodeProvided(int vonageCode)
    {
        int userId = await _serviceFacade.usuarioService.GetUserIdFromTokenAsync();
        Usuario dbExistingUserCredentials = await _serviceFacade.usuarioService.GetByIdAsync(userId);

        bool code = _serviceFacade.vonageSmsService.VerifyCode(vonageCode);

        if (code == false) return BadRequest("El codigo ingresado no coincide con el que se le envio por sms.");

        dbExistingUserCredentials.VerificadoSms = true;

        Usuario modifiedCredentials = await _serviceFacade.usuarioService.Update(userId, dbExistingUserCredentials);

        return Ok("Se verifico su numero de telefono correctamente" + code);
    }

    [HttpGet("obtener-usuario-entradas-reventa")]
    public async Task<IActionResult> GetUserResales()
    {
        try
        {
           int userId = await _serviceFacade.usuarioService.GetUserIdFromTokenAsync();

            List<Entrada> resales = await _serviceFacade.entradaService.GetTicketsInResaleByUserIdAsync(userId);

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
            int userId = await _serviceFacade.usuarioService.GetUserIdFromTokenAsync();

            List < HistorialCompraVenta> resales = await _serviceFacade.reventaService.GetBoughtTicketsByCompradorIdAsync(userId);

            return Ok(resales);
        }
        catch (Exception)
        {

            return BadRequest("No tenes entradas compradas.");
        }
    }
}
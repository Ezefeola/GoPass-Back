using GoPass.Application.Facades.ServiceFacade;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Request.UserRequestDTOs;
using GoPass.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GoPass.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuarioController : ControllerBase
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly ICustomAutoMapper customAutoMapper;
    private readonly IServiceFacade _serviceFacade;

    public UsuarioController(ILogger<UsuarioController> logger, 
            ICustomAutoMapper customAutoMapper,
            IServiceFacade serviceFacade
        )
    {
        _logger = logger;
        this.customAutoMapper = customAutoMapper;
        _serviceFacade = serviceFacade;
    }

    [Authorize]
    [HttpGet("user-credentials")]
    public async Task<IActionResult> GetUserCredentials()
    {
        int userId = await _serviceFacade.AuthService.GetUserIdFromTokenAsync();
        Usuario dbExistingUserCredentials = await _serviceFacade.UsuarioService.GetByIdAsync(userId);

        dbExistingUserCredentials.DNI = _serviceFacade.AesGcmCryptoService.Decrypt(dbExistingUserCredentials.DNI!);
        dbExistingUserCredentials.NumeroTelefono = _serviceFacade.AesGcmCryptoService.Decrypt(dbExistingUserCredentials.NumeroTelefono!);

        return Ok(dbExistingUserCredentials);
    }

    [Authorize]
    [HttpPut("modify-user-credentials")]
    public async Task<IActionResult> ModifyUserCredentials(ModifyUsuarioRequestDto modifyUsuarioRequestDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            int userId = await _serviceFacade.AuthService.GetUserIdFromTokenAsync();
            Usuario dbExistingUserCredentials = await _serviceFacade.UsuarioService.GetByIdAsync(userId);

            //Usuario credentialsToModify = modifyUsuarioRequestDto.MapToModel(dbExistingUserCredentials);
            var stopwatch = Stopwatch.StartNew();
            Usuario credentialsToModify = customAutoMapper.Map(modifyUsuarioRequestDto, dbExistingUserCredentials);
            stopwatch.Stop();
            Console.WriteLine($"El mapeo tomo: {stopwatch.ElapsedMilliseconds} ms");

            credentialsToModify.Verificado = true;
            Usuario modifiedCredentials = await _serviceFacade.UsuarioService.ModifyUserCredentialsAsync(userId, credentialsToModify, cancellationToken);

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
        var result = await _serviceFacade.VonageSmsService.SendVonageVerificationCode(phoneNumber);

        if (result)
        {
            return Ok(new { message = "Código de verificación enviado exitosamente." });
        }

        return BadRequest(new { message = "Error al enviar el código de verificación." });
    }

    [HttpPost("verify-provided-code")]
    public async Task<IActionResult> VerifyVonageCodeProvided(VerifyVonageCodeRequestDto verifyVonageCodeRequestDto, CancellationToken cancellationToken)
    {
        int userId = await _serviceFacade.AuthService.GetUserIdFromTokenAsync();
        Usuario dbExistingUserCredentials = await _serviceFacade.UsuarioService.GetByIdAsync(userId);

        //bool code = _serviceFacade.VonageSmsService.VerifyCode(verifyVonageCodeRequestDto.VonageCode);

        //if (code == false) return BadRequest("El codigo ingresado no coincide con el que se le envio por sms.");

        dbExistingUserCredentials.VerificadoSms = true;
        Usuario modifiedCredentials = await _serviceFacade.UsuarioService.UpdateAsync(userId, dbExistingUserCredentials, cancellationToken);

        return Ok("Se verifico su numero de telefono correctamente");
    }

    [HttpGet("obtener-usuario-entradas-reventa")]
    public async Task<IActionResult> GetUserResales()
    {
        try
        {
           int userId = await _serviceFacade.AuthService.GetUserIdFromTokenAsync();

            List<Entrada> resales = await _serviceFacade.EntradaService.GetTicketsInResaleByUserIdAsync(userId);

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
            int userId = await _serviceFacade.AuthService.GetUserIdFromTokenAsync();

            List < HistorialCompraVenta> resales = await _serviceFacade.ReventaService.GetBoughtTicketsByCompradorIdAsync(userId);

            return Ok(resales);
        }
        catch (Exception)
        {

            return BadRequest("No tenes entradas compradas.");
        }
    }
}
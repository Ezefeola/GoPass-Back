using GoPass.Application.ServiceFacade;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoPass.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IServiceFacade _serviceFacade;

    public AuthController(IServiceFacade serviceFacade)
    {
        _serviceFacade = serviceFacade;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Usuario userToRegister = registerRequestDto.FromRegisterToModel();

            Usuario registeredUser = await _serviceFacade.usuarioService.RegisterUserAsync(userToRegister);

            if (registeredUser is null) BadRequest("El usuario es nulo " + registeredUser);

            string confirmationUrl = $"{Request.Scheme}://{Request.Host}/api/Auth/confirmar-cuenta?token={registeredUser.Token}";

            var valoresReemplazo = new Dictionary<string, string>
             {
                 { "Nombre", registeredUser.Nombre! },
                 { "UrlConfirmacion", confirmationUrl }
             };
            string contenidoPlantilla = await _serviceFacade.templateService.ObtenerContenidoTemplateAsync("VerifyEmail", valoresReemplazo);
            string emailSubject = "Confirmacion de cuenta";
            EmailValidationRequestDto emailConfig = new();
            EmailValidationRequestDto emailToSend = emailConfig.AssignEmailValues(userToRegister.Email, emailSubject, contenidoPlantilla);
            bool enviado = await _serviceFacade.emailService.SendVerificationEmailAsync(emailToSend);

            return Ok(registeredUser);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Usuario userToLogin = loginRequestDto.FromLoginToModel();

            Usuario logUser = await _serviceFacade.usuarioService.AuthenticateAsync(userToLogin.Email, userToLogin.Password);

            if (!logUser.VerificadoEmail) return BadRequest("Falta confirmar la cuenta verifiquela en su correo electronico");

            return Ok(logUser.FromModelToLoginResponse());
        }
        catch (Exception ex)
        {
            return Unauthorized("Las credenciales no son válidas.");
        }
    }

    [HttpGet("confirmar-cuenta")]
    public async Task<IActionResult> ConfirmarCuenta([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("Token es nulo o está vacío.");
        }

        try
        {
            string userIdObtainedString = await _serviceFacade.usuarioService.CleanTokenAsync(token);
            int userIdParsed = int.Parse(userIdObtainedString);

            if (userIdParsed <= 0)
            {
                return BadRequest("ID de usuario no válido.");
            }

            var user = await _serviceFacade.usuarioService.GetByIdAsync(userIdParsed);
            if (user is null)
            {
                return NotFound("No se encontró el usuario.");
            }

            user.VerificadoEmail = true;
            await _serviceFacade.usuarioService.Update(user.Id, user);

            return Ok("Cuenta confirmada exitosamente.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error interno del servidor.");
        }
    }

    [HttpPost("solicitar-restablecimiento")]
    public async Task<IActionResult> SolicitarRestablecimiento([FromBody] PasswordResetRequestDto passwordResetRequestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var usuario = await _serviceFacade.usuarioService.GetUserByEmailAsync(passwordResetRequestDto.Email);
            if (usuario == null)
            {
                return NotFound("No se encontró un usuario con ese correo.");
            }
            if (usuario.VerificadoEmail is false)
                return BadRequest("No se pudo solicitar el restablecimiento de contraseña sin haber realizado antes la validacion en el correo electronico.");

            usuario.Restablecer = true;
            await _serviceFacade.usuarioService.Update(usuario.Id, usuario);

            string resetUrl = $"{Request.Scheme}://{Request.Host}/api/Usuario/restablecer-actualizar?email={usuario.Email}";

            var valoresReemplazo = new Dictionary<string, string>

            {
                { "Nombre", usuario.Nombre! },
                { "UrlRestablecimiento", resetUrl }
            };

            string contenidoPlantilla = await _serviceFacade.templateService.ObtenerContenidoTemplateAsync("ResetPassword", valoresReemplazo);
            string emailSubject = "Restablecimiento de contraseña";

            EmailValidationRequestDto emailConfig = new();
            EmailValidationRequestDto emailToSend = emailConfig.AssignEmailValues(usuario.Email, emailSubject, contenidoPlantilla);

            bool sent = await _serviceFacade.emailService.SendVerificationEmailAsync(emailToSend);

            if (!sent)
            {
                return BadRequest("No se pudo enviar el correo para restablecer la contraseña");
            }
            return Ok("Se envio el correo para restablecer la contraseña, revise su bandeja de entrada o en la carpeta spam");
        }
        catch (ArgumentException argEx)
        {
            return BadRequest(argEx.Message);
        }
    }

    [HttpPost("restablecer-actualizar")]
    public async Task<IActionResult> RestablecerActualizar([FromBody] ConfirmPasswordResetRequestDto confirmPasswordResetRequestDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var usuario = await _serviceFacade.usuarioService.GetUserByEmailAsync(confirmPasswordResetRequestDto.Email);
            if (usuario.Restablecer is false)
                return BadRequest("Usted no ha solicitado un restablecimiento de contraseña");

            var actualizado = await _serviceFacade.usuarioService.ConfirmResetPasswordAsync(false, confirmPasswordResetRequestDto.Password,
                confirmPasswordResetRequestDto.Email, cancellationToken);

            if (actualizado)
            {
                return Ok(new { mensaje = "Contraseña actualizada con éxito." });
            }
            else
            {
                return BadRequest(new { mensaje = "No se pudo actualizar la contraseña. Verifica el token o el estado de la solicitud." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error interno del servidor." });
        }
    }

}

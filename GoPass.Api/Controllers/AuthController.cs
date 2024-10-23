using GoPass.Application.ServiceFacade;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GoPass.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IServiceFacade _serviceFacade;
    private readonly ICustomAutoMapper customAutoMapper;

    public AuthController(IServiceFacade serviceFacade, ICustomAutoMapper customAutoMapper)
    {
        _serviceFacade = serviceFacade;
        this.customAutoMapper = customAutoMapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Usuario userToRegister = registerRequestDto.MapToModel();

            Usuario registeredUser = await _serviceFacade.UsuarioService.RegisterUserAsync(userToRegister);

            if (registeredUser is null) BadRequest("El usuario es nulo " + registeredUser);

            string confirmationUrl = $"{Request.Scheme}://{Request.Host}/api/Auth/confirmar-cuenta?token={registeredUser!.Token}";

            var valoresReemplazo = new Dictionary<string, string>
             {
                 { "Nombre", registeredUser.Nombre! },
                 { "UrlConfirmacion", confirmationUrl }
             };
            string contenidoPlantilla = await _serviceFacade.TemplateService.ObtenerContenidoTemplateAsync("VerifyEmail", valoresReemplazo);
            string emailSubject = "Confirmacion de cuenta";
            EmailValidationRequestDto emailConfig = new();
            EmailValidationRequestDto emailToSend = emailConfig.AssignEmailValues(userToRegister.Email, emailSubject, contenidoPlantilla);
            bool enviado = await _serviceFacade.EmailService.SendVerificationEmailAsync(emailToSend);

            return Ok(registeredUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Usuario userToLogin = loginRequestDto.MapToModel();

            Usuario logUser = await _serviceFacade.UsuarioService.AuthenticateAsync(userToLogin.Email, userToLogin.Password);

            if (!logUser.VerificadoEmail) return BadRequest("Falta confirmar la cuenta verifiquela en su correo electronico");

            var stopwatch = Stopwatch.StartNew();
            LoginResponseDto loginResponseDto = customAutoMapper.Map<Usuario, LoginResponseDto>(logUser);
            stopwatch.Stop();
            Console.WriteLine($"El mapeo tomo: {stopwatch.ElapsedMilliseconds} ms");

            return Ok(loginResponseDto);
        }
        catch (Exception)
        {
            return Unauthorized("Las credenciales no son válidas.");
        }
    }

    [HttpGet("confirmar-cuenta")]
    public async Task<IActionResult> ConfirmarCuenta([FromQuery] string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("Token es nulo o está vacío.");
        }

        try
        {
            string userIdObtainedString = await _serviceFacade.UsuarioService.CleanTokenAsync(token);
            int userIdParsed = int.Parse(userIdObtainedString);

            if (userIdParsed <= 0)
            {
                return BadRequest("ID de usuario no válido.");
            }

            var user = await _serviceFacade.UsuarioService.GetByIdAsync(userIdParsed);
            if (user is null)
            {
                return NotFound("No se encontró el usuario.");
            }

            user.VerificadoEmail = true;
            await _serviceFacade.UsuarioService.UpdateAsync(user.Id, user, cancellationToken);

            return Ok("Cuenta confirmada exitosamente.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Error interno del servidor.");
        }
    }

    [HttpPost("solicitar-restablecimiento")]
    public async Task<IActionResult> SolicitarRestablecimiento([FromBody] PasswordResetRequestDto passwordResetRequestDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var usuario = await _serviceFacade.UsuarioService.GetUserByEmailAsync(passwordResetRequestDto.Email);
            if (usuario == null)
            {
                return NotFound("No se encontró un usuario con ese correo.");
            }
            if (usuario.VerificadoEmail is false)
                return BadRequest("No se pudo solicitar el restablecimiento de contraseña sin haber realizado antes la validacion en el correo electronico.");

            usuario.Restablecer = true;
            await _serviceFacade.UsuarioService.UpdateAsync(usuario.Id, usuario, cancellationToken);

            string resetUrl = $"{Request.Scheme}://{Request.Host}/api/Usuario/restablecer-actualizar?email={usuario.Email}";

            var valoresReemplazo = new Dictionary<string, string>

            {
                { "Nombre", usuario.Nombre! },
                { "UrlRestablecimiento", resetUrl }
            };

            string contenidoPlantilla = await _serviceFacade.TemplateService.ObtenerContenidoTemplateAsync("ResetPassword", valoresReemplazo);
            string emailSubject = "Restablecimiento de contraseña";

            EmailValidationRequestDto emailConfig = new();
            EmailValidationRequestDto emailToSend = emailConfig.AssignEmailValues(usuario.Email, emailSubject, contenidoPlantilla);

            bool sent = await _serviceFacade.EmailService.SendVerificationEmailAsync(emailToSend);

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
            var usuario = await _serviceFacade.UsuarioService.GetUserByEmailAsync(confirmPasswordResetRequestDto.Email);
            if (usuario.Restablecer is false)
                return BadRequest("Usted no ha solicitado un restablecimiento de contraseña");

            var actualizado = await _serviceFacade.UsuarioService.ConfirmResetPasswordAsync(false, confirmPasswordResetRequestDto.Password,
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
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error interno del servidor." });
        }
    }

}

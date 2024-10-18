using GoPass.Application.Services.Classes;
using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Application.Validators.Users;
using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoPass.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEntradaService _entradaService;
        private readonly IReventaService _reventaService;
        private readonly IAesGcmCryptoService _aesGcmCryptoService;
        private readonly IVonageSmsService _vonageSmsService;
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger, IUsuarioService usuarioService, IEntradaService entradaService, IReventaService reventaService,
            IAesGcmCryptoService aesGcmCryptoService, IVonageSmsService vonageSmsService, IEmailService emailService, ITemplateService templateService)
        {
            _usuarioService = usuarioService;
            _entradaService = entradaService;
            _reventaService = reventaService;
            _aesGcmCryptoService = aesGcmCryptoService;
            _vonageSmsService = vonageSmsService;
            _emailService = emailService;
            _templateService = templateService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
 
            try
            {
                Usuario userToRegister = registerRequestDto.FromRegisterToModel();

                Usuario registeredUser = await _usuarioService.RegisterUserAsync(userToRegister);

                if (registeredUser is null) BadRequest("El usuario es nulo " + registeredUser);

                string confirmationUrl = $"{Request.Scheme}://{Request.Host}/api/Usuario/confirmar-cuenta?token={registeredUser.Token}";

                var valoresReemplazo = new Dictionary<string, string>
                 {
                     { "Nombre", registeredUser.Nombre! },
                     { "UrlConfirmacion", confirmationUrl }
                 };
                string contenidoPlantilla = await _templateService.ObtenerContenidoTemplateAsync("VerifyEmail", valoresReemplazo);
                string emailSubject = "Confirmacion de cuenta";
                EmailValidationRequestDto emailConfig = new();
                EmailValidationRequestDto emailToSend = emailConfig.AssignEmailValues(userToRegister.Email, emailSubject, contenidoPlantilla);
                bool enviado = await _emailService.SendVerificationEmailAsync(emailToSend);

                return Ok(registeredUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el usuario.");
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                Usuario userToLogin = loginRequestDto.FromLoginToModel();

                Usuario logUser = await _usuarioService.AuthenticateAsync(userToLogin.Email, userToLogin.Password);

                if (!logUser.VerificadoEmail) return BadRequest("Falta confirmar la cuenta verifiquela en su correo electronico");

                return Ok(logUser.FromModelToLoginResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al autenticar el usuario.");
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
                _logger.LogInformation($"Token recibido para confirmación: {token}");

                int userIdParsed = await _usuarioService.CleanTokenAsync(token);
                _logger.LogInformation($"ID de usuario obtenido del token: {userIdParsed}");

                if (userIdParsed <= 0)
                {
                    _logger.LogWarning("ID de usuario no válido.");
                    return BadRequest("ID de usuario no válido.");
                }

                var user = await _usuarioService.GetByIdAsync(userIdParsed);
                if (user is null)
                {
                    return NotFound("No se encontró el usuario.");
                }

                user.VerificadoEmail = true;
                await _usuarioService.Update(user.Id, user);

                return Ok("Cuenta confirmada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al confirmar la cuenta.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        [HttpPost("solicitar-restablecimiento")]
        public async Task<IActionResult> SolicitarRestablecimiento([FromBody] PasswordResetRequestDto passwordResetRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                var usuario = await _usuarioService.GetUserByEmailAsync(passwordResetRequestDto.Email);
                if (usuario == null)
                {
                    return NotFound("No se encontró un usuario con ese correo.");
                }
                if (usuario.VerificadoEmail is false) 
                    return BadRequest("No se pudo solicitar el restablecimiento de contraseña sin haber realizado antes la validacion en el correo electronico.");

                usuario.Restablecer = true;
                await _usuarioService.Update(usuario.Id, usuario);

                string resetUrl = $"{Request.Scheme}://{Request.Host}/api/Usuario/restablecer-actualizar?email={usuario.Email}";

                var valoresReemplazo = new Dictionary<string, string>

                {
                    { "Nombre", usuario.Nombre },
                    { "UrlRestablecimiento", resetUrl }
                };

                string contenidoPlantilla = await _templateService.ObtenerContenidoTemplateAsync("ResetPassword", valoresReemplazo);
                string emailSubject = "Restablecimiento de contraseña";

                EmailValidationRequestDto emailConfig = new();
                EmailValidationRequestDto emailToSend = emailConfig.AssignEmailValues(usuario.Email, emailSubject, contenidoPlantilla);

                bool sent = await _emailService.SendVerificationEmailAsync(emailToSend);

                if (!sent)
                {
                    return BadRequest("No se pudo enviar el correo para restablecer la contraseña");
                }
                return Ok("Se envio el correo para restablecer la contraseña, revise su bandeja de entrada o en la carpeta spam");
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Error al solicitar el restablecimiento de contraseña: " + argEx.Message);
                return BadRequest(argEx.Message);
            }
        }

        [HttpPost("restablecer-actualizar")]
        public async Task<IActionResult> RestablecerActualizar([FromBody] ConfirmPasswordResetRequestDto confirmPasswordResetRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuario = await _usuarioService.GetUserByEmailAsync(confirmPasswordResetRequestDto.Email);
                if (usuario.Restablecer is false) 
                    return BadRequest("Usted no ha solicitado un restablecimiento de contraseña");

                var actualizado = await _usuarioService.ConfirmResetPasswordAsync(false, confirmPasswordResetRequestDto.Password, 
                    confirmPasswordResetRequestDto.Email);

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
                _logger.LogError(ex, "Error al actualizar la contraseña.");
                return StatusCode(500, new { mensaje = "Error interno del servidor." });
            }
        }

        [Authorize]
        [HttpGet("user-credentials")]
        public async Task<IActionResult> GetUserCredentials()
        {
            int userId = await _usuarioService.GetUserIdFromTokenAsync();
            Usuario dbExistingUserCredentials = await _usuarioService.GetByIdAsync(userId);

            dbExistingUserCredentials.DNI = _aesGcmCryptoService.Decrypt(dbExistingUserCredentials.DNI!);
            dbExistingUserCredentials.NumeroTelefono = _aesGcmCryptoService.Decrypt(dbExistingUserCredentials.NumeroTelefono!);

            return Ok(dbExistingUserCredentials);
        }

        [Authorize]
        [HttpPut("modify-user-credentials")]
        public async Task<IActionResult> ModifyUserCredentials(ModifyUsuarioRequestDto modifyUsuarioRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                int userId = await _usuarioService.GetUserIdFromTokenAsync();
                Usuario dbExistingUserCredentials = await _usuarioService.GetByIdAsync(userId);

                if (await _usuarioService.VerifyDniExistsAsync(modifyUsuarioRequestDto.DNI, userId))
                {
                    return BadRequest("El DNI ya se encuentra registrado por otro usuario.");
                }

                if (await _usuarioService.VerifyPhoneNumberExistsAsync(modifyUsuarioRequestDto.NumeroTelefono, userId))
                {
                    return BadRequest("El número de teléfono ya se encuentra registrado por otro usuario.");
                }

                Usuario credentialsToModify = modifyUsuarioRequestDto.FromModifyUsuarioRequestToModel(dbExistingUserCredentials);

                credentialsToModify.DNI = _aesGcmCryptoService.Encrypt(credentialsToModify.DNI!);
                credentialsToModify.NumeroTelefono = _aesGcmCryptoService.Encrypt(credentialsToModify.NumeroTelefono!);

                if(credentialsToModify.DNI is not null && credentialsToModify.Nombre is not null && credentialsToModify.NumeroTelefono is not null)
                {
                    credentialsToModify.Verificado = true;
                }

                Usuario modifiedCredentials = await _usuarioService.Update(userId, credentialsToModify);

                modifiedCredentials.DNI = _aesGcmCryptoService.Decrypt(credentialsToModify.DNI!);
                modifiedCredentials.NumeroTelefono = _aesGcmCryptoService.Decrypt(credentialsToModify.NumeroTelefono!);
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
            var result = await _vonageSmsService.SendVonageVerificationCode(phoneNumber);

            if (result)
            {
                return Ok(new { message = "Código de verificación enviado exitosamente." });
            }

            return BadRequest(new { message = "Error al enviar el código de verificación." });
        }

        [HttpPost("verify-provided-code")]
        public async Task<IActionResult> VerifyVonageCodeProvided(int vonageCode)
        {
            int userId = await _usuarioService.GetUserIdFromTokenAsync();
            Usuario dbExistingUserCredentials = await _usuarioService.GetByIdAsync(userId);

            bool code = _vonageSmsService.VerifyCode(vonageCode);

            if (code == false) return BadRequest("El codigo ingresado no coincide con el que se le envio por sms.");

            dbExistingUserCredentials.VerificadoSms = true;

            Usuario modifiedCredentials = await _usuarioService.Update(userId, dbExistingUserCredentials);

            return Ok("Se verifico su numero de telefono correctamente" + code);
        }

        [HttpGet("obtener-usuario-entradas-reventa")]
        public async Task<IActionResult> GetUserResales()
        {
            try
            {
               int userId = await _usuarioService.GetUserIdFromTokenAsync();

                List<Entrada> resales = await _entradaService.GetTicketsInResaleByUserIdAsync(userId);

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
                int userId = await _usuarioService.GetUserIdFromTokenAsync();

                List < HistorialCompraVenta> resales = await _reventaService.GetBoughtTicketsByCompradorIdAsync(userId);

                return Ok(resales);
            }
            catch (Exception)
            {

                return BadRequest("No tenes entradas compradas.");
            }
        }
    }
}
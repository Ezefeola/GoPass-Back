using GoPass.Application.Services.Interfaces;
using GoPass.Domain.Models;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GoPass.Application.Services.Classes
{
    public class UsuarioService : GenericService<Usuario>, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IAesGcmCryptoService _aesGcmCryptoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        public UsuarioService(IUsuarioRepository usuarioRepository, ITokenService tokenService, IAesGcmCryptoService aesGcmCryptoService, IHttpContextAccessor httpContextAccessor) : base(usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _aesGcmCryptoService = aesGcmCryptoService;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = new PasswordHasher<Usuario>();    
        }
        public async Task<List<Usuario>> GetAllUsersWithRelationsAsync()
        {
            var users = await _usuarioRepository.GetAllUsersWithRelations();

            return users;
        }

        public async Task<Usuario> GetUserByEmailAsync(string email)
        {
            return await _usuarioRepository.GetUserByEmail(email);
        }

        public async Task<Usuario> DeleteUserWithRelationsAsync(int id)
        {
            var deletedUser = await _usuarioRepository.DeleteUserWithRelations(id);

            return deletedUser;
        }

        public async Task<Usuario> RegisterUserAsync(Usuario usuario)
        {
            usuario.Password = _passwordHasher.HashPassword(usuario, usuario.Password);

            var nuevoUsuario = await _usuarioRepository.Create(usuario);

            if (nuevoUsuario.Id <= 0)
            {
                throw new Exception("El ID del usuario no es válido después de la creación.");
            }

            var userToken = _tokenService.CreateToken(nuevoUsuario);
            nuevoUsuario.Token = userToken; 
            await _usuarioRepository.StorageToken(usuario.Id, userToken);
            return nuevoUsuario; 

        }

        public async Task<Usuario> AuthenticateAsync(string email, string password)
        {
            Usuario userInDb = await _usuarioRepository.GetUserByEmail(email);

            PasswordVerificationResult passwordVerification = _passwordHasher.VerifyHashedPassword(userInDb, userInDb.Password, password);

            if (passwordVerification == PasswordVerificationResult.Failed) throw new Exception("Las credenciales no son correctas");

            Usuario user = await _usuarioRepository.AuthenticateUser(email, password);

            string token = _tokenService.CreateToken(user);
            user.Token = token;

            return user;
        }

        public async Task<bool> VerifyEmailExistsAsync(string email)
        {
            bool userEmail = await _usuarioRepository.VerifyEmailExists(email);

            return userEmail!;
        }

        public async Task<bool> VerifyDniExistsAsync(string dni, int userId)
        {
            string encriptedDni = _aesGcmCryptoService.Encrypt(dni);
            bool userDni = await _usuarioRepository.VerifyDniExists(encriptedDni, userId);

            return userDni;
        }
        public async Task<bool> VerifyPhoneNumberExistsAsync(string phoneNumber, int userId)
        {
            string encriptedPhoneNumber = _aesGcmCryptoService.Encrypt(phoneNumber);
            bool userPhoneNumber = await _usuarioRepository.VerifyPhoneNumberExists(encriptedPhoneNumber, userId);

            return userPhoneNumber;
        }

        public async Task<int> GetUserIdFromTokenAsync()
        {
            string authHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString();
            string userId = await CleanTokenAsync(authHeader);

            return int.Parse(userId);
        }
        public async Task<string> CleanTokenAsync(string token)
        {
            string cleanToken = token.StartsWith("Bearer ") ? token.Substring("Bearer ".Length) : token;

            if (string.IsNullOrWhiteSpace(cleanToken))
            {
                throw new Exception("Token nulo o vacío.");
            }

            string decodedToken = await _tokenService.DecodeToken(cleanToken!);

            return decodedToken;
        }

        public async Task<bool> ConfirmResetPasswordAsync(bool reset, string newPassword, string userEmail)
        {
            try
            {
                var usuario = await _usuarioRepository.GetUserByEmail(userEmail);
                
                if (usuario == null)
                {
                    return false;
                }

                usuario.Restablecer = reset;
                usuario.Password = _passwordHasher.HashPassword(usuario, newPassword);

                await _usuarioRepository.Update(usuario.Id, usuario);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ValidateUserCredentialsToPublishTicket(int userId)
        {
            bool isvalid = true;

            Usuario usuario = await _usuarioRepository.GetById(userId);

            if (string.IsNullOrEmpty(usuario.Nombre) ||
            string.IsNullOrEmpty(usuario.DNI) ||
            string.IsNullOrEmpty(usuario.NumeroTelefono) ||
            !usuario.VerificadoEmail ||
            !usuario.VerificadoSms)
            {
                return isvalid = false;
            }

            return isvalid;
        }
    }
}

using GoPass.Application.Services.Interfaces;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GoPass.Application.Services.Classes
{
    public class AuthService : GenericService<Usuario>, IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAesGcmCryptoService _aesGcmCryptoService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        public AuthService(IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IAesGcmCryptoService aesGcmCryptoService,
            ITokenService tokenService
        ) : base(unitOfWork.UsuarioRepository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _aesGcmCryptoService = aesGcmCryptoService;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<Usuario>();
        }
        public async Task<Usuario> RegisterUserAsync(Usuario usuario, CancellationToken cancellationToken)
        {
            usuario.Password = _passwordHasher.HashPassword(usuario, usuario.Password);

            Usuario newUser = await _unitOfWork.UsuarioRepository.Create(usuario);
            await _unitOfWork.Complete(cancellationToken);

            if (newUser.Id <= 0)
            {
                throw new Exception("El ID del usuario no es válido después de la creación.");
            }

            var userToken = _tokenService.CreateToken(newUser);
            newUser.Token = userToken;
            await _unitOfWork.UsuarioRepository.StorageToken(usuario.Id, userToken);

            return newUser;

        }

        public async Task<Usuario> AuthenticateAsync(string email, string password)
        {
            Usuario userInDb = await _unitOfWork.UsuarioRepository.GetUserByEmail(email);

            PasswordVerificationResult passwordVerification = _passwordHasher.VerifyHashedPassword(userInDb, userInDb.Password, password);

            if (passwordVerification == PasswordVerificationResult.Failed) throw new Exception("Las credenciales no son correctas");

            Usuario user = await _unitOfWork.UsuarioRepository.AuthenticateUser(email, password);

            string token = _tokenService.CreateToken(user);
            user.Token = token;

            return user;
        }

        public async Task<bool> ConfirmResetPasswordAsync(bool reset, string newPassword, string userEmail, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await _unitOfWork.UsuarioRepository.GetUserByEmail(userEmail);

                if (usuario == null)
                {
                    return false;
                }

                usuario.Restablecer = reset;
                usuario.Password = _passwordHasher.HashPassword(usuario, newPassword);

                await _unitOfWork.UsuarioRepository.Update(usuario.Id, usuario);

                await _unitOfWork.Complete(cancellationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> GetUserIdFromTokenAsync()
        {
            string authHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString();
            string userId = await _tokenService.CleanTokenAsync(authHeader);

            return int.Parse(userId);
        }
    }
}

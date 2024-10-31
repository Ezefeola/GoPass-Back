using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GoPass.Application.Services.Classes
{
    public class AuthService : GenericService<User>, IAuthService
    {
        private readonly ISmappfter _smappfter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAesGcmCryptoService _aesGcmCryptoService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;
        public AuthService
        (
            ISmappfter smappfter,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IAesGcmCryptoService aesGcmCryptoService,
            ITokenService tokenService
        ) : base(unitOfWork.UserRepository, unitOfWork)
        {
            _smappfter = smappfter;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _aesGcmCryptoService = aesGcmCryptoService;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<User>();
        }
        public async Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto registerRequestDto, CancellationToken cancellationToken)
        {
            User user = _smappfter.Map<RegisterRequestDto, User>(registerRequestDto);
            user.Password = _passwordHasher.HashPassword(user, user.Password);

            User newUser = await _unitOfWork.AuthRepository.Create(user, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);

            RegisterResponseDto registerResponseDto = _smappfter.Map<User, RegisterResponseDto>(newUser);
            string userToken = _tokenService.CreateToken(newUser);
            registerResponseDto.Token = userToken;

            return registerResponseDto;

        }

        public async Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto loginRequestDto)
        {
            User userData = _smappfter.Map<LoginRequestDto, User>(loginRequestDto);
            User userInDb = await _unitOfWork.AuthRepository.AuthenticateUser(userData);
            PasswordVerificationResult passwordVerification = _passwordHasher.VerifyHashedPassword(userInDb, userInDb.Password, userData.Password);

            if (passwordVerification == PasswordVerificationResult.Failed) throw new Exception("Las credenciales no son correctas");

            LoginResponseDto loginResponseDto = _smappfter.Map<User, LoginResponseDto>(userInDb);
            string token = _tokenService.CreateToken(userInDb);
            loginResponseDto.Token = token;

            return loginResponseDto;
        }

        public async Task<bool> ConfirmResetPasswordAsync(bool isReset, string newPassword, string userEmail, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await _unitOfWork.UserRepository.GetUserByEmail(userEmail);

                if (usuario == null)
                {
                    return false;
                }

                usuario.IsReset = isReset;
                usuario.Password = _passwordHasher.HashPassword(usuario, newPassword);

                await _unitOfWork.UserRepository.Update(usuario.Id, usuario);

                await _unitOfWork.Complete(cancellationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetUserIdFromToken()
        {
            string authHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString();
            string userId = _tokenService.CleanToken(authHeader);

            return int.Parse(userId);
        }
    }
}

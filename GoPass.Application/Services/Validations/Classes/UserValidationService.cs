using GoPass.Application.Services.Classes;
using GoPass.Application.Services.Interfaces;
using GoPass.Application.Services.Validations.Interfaces;
using GoPass.Infrastructure.UnitOfWork;

namespace GoPass.Application.Services.Validations.Classes
{
    public class UserValidationService : IUserValidationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVonageSmsService vonageSmsService;
        private readonly IAesGcmCryptoService aesGcmCryptoService;
        private readonly IAuthService authService;

        public UserValidationService(IUnitOfWork unitOfWork, 
            IVonageSmsService vonageSmsService, 
            IAesGcmCryptoService aesGcmCryptoService,
            IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            this.vonageSmsService = vonageSmsService;
            this.aesGcmCryptoService = aesGcmCryptoService;
            this.authService = authService;
        }

        public async Task<bool> VerifyEmailExistsAsync(string email)
        {
            bool userEmail = await _unitOfWork.UsuarioRepository.VerifyEmailExists(email);

            return userEmail!;
        }

        public async Task<bool> VerifyDniExistsAsync(string dni)
        {
            int userId = await authService.GetUserIdFromTokenAsync();
            string encriptedDni = aesGcmCryptoService.Encrypt(dni);
            bool userDni = await _unitOfWork.UsuarioRepository.VerifyDniExists(encriptedDni, userId);

            return userDni;
        }
        public async Task<bool> VerifyPhoneNumberExistsAsync(string phoneNumber)
        {
            int userId = await authService.GetUserIdFromTokenAsync();
            string encriptedPhoneNumber = aesGcmCryptoService.Encrypt(phoneNumber);
            bool userPhoneNumber = await _unitOfWork.UsuarioRepository.VerifyPhoneNumberExists(encriptedPhoneNumber, userId);

            return userPhoneNumber;
        }

        public async Task<bool> VerifyEnteredCodeAsync(int vonageCode)
        {

            bool isValid = await vonageSmsService.VerifyCode(vonageCode);

            return isValid;
        }
    }
}

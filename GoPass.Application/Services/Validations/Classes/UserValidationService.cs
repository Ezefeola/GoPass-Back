using GoPass.Application.Services.Interfaces;
using GoPass.Application.Services.Validations.Interfaces;
using GoPass.Infrastructure.UnitOfWork;

namespace GoPass.Application.Services.Validations.Classes
{
    public class UserValidationService : IUserValidationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService emailService;
        private readonly IAesGcmCryptoService aesGcmCryptoService;
        private readonly IAuthService authService;

        public UserValidationService(
            IUnitOfWork unitOfWork, 
            IEmailService emailService,
            IAesGcmCryptoService aesGcmCryptoService,
            IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            this.emailService = emailService;
            this.aesGcmCryptoService = aesGcmCryptoService;
            this.authService = authService;
        }

        public async Task<bool> VerifyEmailExistsAsync(string email)
        {
            bool userEmail = await _unitOfWork.UserRepository.VerifyEmailExists(email);

            return userEmail!;
        }

        public async Task<bool> VerifyDniExistsAsync(string dni)
        {
            int userId = authService.GetUserIdFromToken();
            string encriptedDni = aesGcmCryptoService.Encrypt(dni);
            bool userDni = await _unitOfWork.UserRepository.VerifyDniExists(encriptedDni, userId);

            return userDni;
        }
        public async Task<bool> VerifyPhoneNumberExistsAsync(string phoneNumber)
        {
            int userId = authService.GetUserIdFromToken();
            string encriptedPhoneNumber = aesGcmCryptoService.Encrypt(phoneNumber);
            bool userPhoneNumber = await _unitOfWork.UserRepository.VerifyPhoneNumberExists(encriptedPhoneNumber, userId);

            return userPhoneNumber;
        }

        public bool VerifyEnteredCodeAsync(int emailCode)
        {

            bool isValid = emailService.VerifyCode(emailCode);

            return isValid;
        }
    }
}

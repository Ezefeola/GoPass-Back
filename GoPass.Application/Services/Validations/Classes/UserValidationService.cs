using GoPass.Application.ServiceFacade;
using GoPass.Application.Services.Interfaces;
using GoPass.Application.Services.Validations.Interfaces;
using GoPass.Infrastructure.UnitOfWork;

namespace GoPass.Application.Services.Validations.Classes
{
    public class UserValidationService : IUserValidationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioService usuarioService;
        private readonly IVonageSmsService vonageSmsService;
        private readonly IAesGcmCryptoService aesGcmCryptoService;

        public UserValidationService(IUnitOfWork unitOfWork, IUsuarioService usuarioService, IVonageSmsService vonageSmsService, IAesGcmCryptoService aesGcmCryptoService )
        {
            _unitOfWork = unitOfWork;
            this.usuarioService = usuarioService;
            this.vonageSmsService = vonageSmsService;
            this.aesGcmCryptoService = aesGcmCryptoService;
        }

        public async Task<bool> VerifyEmailExistsAsync(string email)
        {
            bool userEmail = await _unitOfWork.UsuarioRepository.VerifyEmailExists(email);

            return userEmail!;
        }

        public async Task<bool> VerifyDniExistsAsync(string dni)
        {
            int userId = await usuarioService.GetUserIdFromTokenAsync();
            string encriptedDni = aesGcmCryptoService.Encrypt(dni);
            bool userDni = await _unitOfWork.UsuarioRepository.VerifyDniExists(encriptedDni, userId);

            return userDni;
        }
        public async Task<bool> VerifyPhoneNumberExistsAsync(string phoneNumber)
        {
            int userId = await usuarioService.GetUserIdFromTokenAsync();
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

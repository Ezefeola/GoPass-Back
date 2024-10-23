namespace GoPass.Application.Services.Validations.Interfaces
{
    public interface IUserValidationService
    {
        Task<bool> VerifyDniExistsAsync(string dni);
        Task<bool> VerifyEmailExistsAsync(string email);
        Task<bool> VerifyPhoneNumberExistsAsync(string phoneNumber);
        Task<bool> VerifyEnteredCodeAsync(int vonageCode);
    }
}
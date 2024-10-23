namespace GoPass.Application.Services.Interfaces;

public interface IVonageSmsService
{
    Task<bool> SendVonageVerificationCode(string phoneNumber);
    Task<bool> VerifyCode(int userInputCode);
}

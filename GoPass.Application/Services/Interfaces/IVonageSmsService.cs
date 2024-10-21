namespace GoPass.Application.Services.Interfaces;

public interface IVonageSmsService
{
    Task<bool> SendVonageVerificationCode(string phoneNumber);
    bool VerifyCode(int userInputCode);
}

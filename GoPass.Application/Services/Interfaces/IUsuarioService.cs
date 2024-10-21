using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface IUsuarioService : IGenericService<Usuario>
{

    Task<List<Usuario>> GetAllUsersWithRelationsAsync();
    Task<Usuario> DeleteUserWithRelationsAsync(int id);
    Task<Usuario> GetUserByEmailAsync(string email);
    Task<Usuario> AuthenticateAsync(string email, string password);
    Task<Usuario> RegisterUserAsync(Usuario usuario);
    Task<int> GetUserIdFromTokenAsync();
    Task<string> CleanTokenAsync(string token);
    Task<bool> VerifyEmailExistsAsync(string email);
    Task<bool> VerifyDniExistsAsync(string dni, int userId);
    Task<bool> VerifyPhoneNumberExistsAsync(string phoneNumber, int userId);
    Task<bool> ConfirmResetPasswordAsync(bool reset, string newPassword, string userEmail, CancellationToken cancellationToken);
    Task<bool> ValidateUserCredentialsToPublishTicket(int userId);
}

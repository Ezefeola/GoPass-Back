using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface IUsuarioService : IGenericService<Usuario>
{

    Task<List<Usuario>> GetAllUsersWithRelationsAsync();
    Task<Usuario> DeleteUserWithRelationsAsync(int id);
    Task<Usuario> GetUserByEmailAsync(string email);
    Task<int> GetUserIdFromTokenAsync();
    Task<string> CleanTokenAsync(string token);
    Task<Usuario> ModifyUserCredentialsAsync(int id, Usuario usuario, CancellationToken cancellationToken);
    Task<bool> ValidateUserCredentialsToPublishTicket(int userId);
    Task<Usuario> AuthenticateAsync(string email, string password);
    Task<Usuario> RegisterUserAsync(Usuario usuario);
    Task<bool> ConfirmResetPasswordAsync(bool reset, string newPassword, string userEmail, CancellationToken cancellationToken);
}

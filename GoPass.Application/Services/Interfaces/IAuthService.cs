using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces
{
    public interface IAuthService : IGenericService<Usuario>
    {
        Task<Usuario> AuthenticateAsync(string email, string password);
        Task<Usuario> RegisterUserAsync(Usuario usuario, CancellationToken cancellationToken);
        Task<bool> ConfirmResetPasswordAsync(bool reset, string newPassword, string userEmail, CancellationToken cancellationToken);
        Task<int> GetUserIdFromTokenAsync();
    }
}
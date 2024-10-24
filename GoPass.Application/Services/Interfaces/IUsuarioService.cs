using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface IUsuarioService : IGenericService<Usuario>
{

    Task<List<Usuario>> GetAllUsersWithRelationsAsync();
    Task<Usuario> DeleteUserWithRelationsAsync(int id);
    Task<Usuario> GetUserByEmailAsync(string email);
    Task<Usuario> ModifyUserCredentialsAsync(int id, Usuario usuario, CancellationToken cancellationToken);
    Task<bool> ValidateUserCredentialsToPublishTicket(int userId);
}

using GoPass.Domain.Models;

namespace GoPass.Infrastructure.Repositories.Interfaces;
public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario> DeleteUserWithRelations(int id);
    Task<List<Usuario>> GetAllUsersWithRelations();
    Task<Usuario> GetUserByToken(string token);
    Task<Usuario> GetUserByEmail(string email);
    Task<Usuario> AuthenticateUser(string email, string password);
    Task<bool> VerifyPhoneNumberExists(string phoneNumber, int userId);
    Task<bool> VerifyDniExists(string dni, int userId);
    Task<bool> VerifyEmailExists(string email);
    Task<int> StorageToken(int userId, string token);
}

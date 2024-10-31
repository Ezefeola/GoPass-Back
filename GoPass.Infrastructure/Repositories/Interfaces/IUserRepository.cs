using GoPass.Domain.Models;

namespace GoPass.Infrastructure.Repositories.Interfaces;
public interface IUserRepository : IGenericRepository<User>
{
    Task<User> DeleteUserWithRelations(int id);
    Task<List<User>> GetAllUsersWithRelations();
    Task<User> GetUserByEmail(string email);
    Task<bool> VerifyPhoneNumberExists(string phoneNumber, int userId);
    Task<bool> VerifyDniExists(string dni, int userId);
    Task<bool> VerifyEmailExists(string email);
}

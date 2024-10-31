using GoPass.Domain.Models;

namespace GoPass.Infrastructure.Repositories.Interfaces
{
    public interface IAuthRepository : IGenericRepository<User>
    {
        Task<User> AuthenticateUser(User user);
    }
}
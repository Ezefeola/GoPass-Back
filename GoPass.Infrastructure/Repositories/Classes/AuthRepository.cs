using GoPass.Domain.Models;
using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoPass.Infrastructure.Repositories.Classes
{
    public class AuthRepository : GenericRepository<User>, IAuthRepository
    {
        public AuthRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<User> AuthenticateUser(User user)
        {
            var userToAuthenticate = await _dbSet.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (userToAuthenticate is null) throw new Exception("Ha habido un error, verifique los campos e intentelo nuevamente");

            return userToAuthenticate;
        }
    }
}

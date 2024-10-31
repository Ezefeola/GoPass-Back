using GoPass.Domain.Models;
using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoPass.Infrastructure.Repositories.Classes;
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    public async Task<List<User>> GetAllUsersWithRelations()
    {
        return await _dbSet
            .Include(x => x.Resales!)
            .ThenInclude(x => x.Ticket)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var emailExists = await _dbSet
            .Where(x => x.Email == email)
            .Select(x => new User 
            {
                Email = x.Email,
                Password = x.Password, 
                IsReset = x.IsReset,
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email);

        return emailExists!;
    }

    public async Task<User> DeleteUserWithRelations(int id)
    {
        var recordToDelete = await GetById(id);

        if (recordToDelete is null) throw new Exception("El registro no se encontro");

        await _dbSet
            .Where(x => x.Id == id)
            .Include(x => x.Tickets!)
            .ThenInclude(x => x.Resale)
            .ExecuteDeleteAsync();

        return recordToDelete;
    }

    public async Task<bool> VerifyEmailExists(string email)
    {
        var userCredentialsExist = await _dbSet.AnyAsync(u => u.Email == email);

        return userCredentialsExist;
    }

    public async Task<bool> VerifyDniExists(string dni, int userId)
    {
        var userDniExist = await _dbSet.AnyAsync(u => u.DNI == dni && u.Id != userId);

        return userDniExist;
    }

    public async Task<bool> VerifyPhoneNumberExists(string phoneNumber, int userId)
    {
        var userPhoneNumberExist = await _dbSet.AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != userId);

        return userPhoneNumberExist;
    }
   
}

using GoPass.Domain.Models;
using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoPass.Infrastructure.Repositories.Classes;
public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Usuario>> GetAllUsersWithRelations()
    {
        return await _dbSet.Include(x => x.Reventa!).ThenInclude(x => x.Entrada).ToListAsync();
    }

    public async Task<Usuario> GetUserByEmail(string email)
    {
        var emailExists = await _dbSet.FirstOrDefaultAsync(x => x.Email == email);

        return emailExists!;
    }

    public async Task<Usuario> DeleteUserWithRelations(int id)
    {
        var recordToDelete = await GetById(id);

        if (recordToDelete is null) throw new Exception("El registro no se encontro");

        await _dbSet.Where(x => x.Id == id).Include(x => x.Entrada!).ThenInclude(x => x.Reventa).ExecuteDeleteAsync();

        return recordToDelete;
    }

    public async Task<Usuario> AuthenticateUser(string email, string password)
    {
        var userToAuthenticate = await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

        if (userToAuthenticate is null) throw new Exception("Ha habido un error, verifique los campos e intentelo nuevamente");

        return userToAuthenticate;
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
        var userPhoneNumberExist = await _dbSet.AnyAsync(u => u.NumeroTelefono == phoneNumber && u.Id != userId);

        return userPhoneNumberExist;
    }

    public async Task<Usuario> GetUserByToken(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Token == token);
    }
    
    public async Task<int> StorageToken(int userId, string token)
    {
        var storeToken = await _dbSet.Where(u => u.Id == userId).ExecuteUpdateAsync(u => u.SetProperty(u => u.Token, token));

        return storeToken;
    }
}

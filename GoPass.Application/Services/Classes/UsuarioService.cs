using GoPass.Application.Services.Interfaces;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GoPass.Application.Services.Classes;

public class UsuarioService : GenericService<Usuario>, IUsuarioService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAesGcmCryptoService _aesGcmCryptoService;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    public UsuarioService(IUnitOfWork unitOfWork, 
            IHttpContextAccessor httpContextAccessor, 
            IAesGcmCryptoService aesGcmCryptoService, 
            ITokenService tokenService 
        ) : base(unitOfWork.UsuarioRepository, unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _aesGcmCryptoService = aesGcmCryptoService;
        _tokenService = tokenService;
        _passwordHasher = new PasswordHasher<Usuario>();    
    }
    public async Task<List<Usuario>> GetAllUsersWithRelationsAsync()
    {
        var users = await _unitOfWork.UsuarioRepository.GetAllUsersWithRelations();

        return users;
    }

    public async Task<Usuario> GetUserByEmailAsync(string email)
    {
        return await _unitOfWork.UsuarioRepository.GetUserByEmail(email);
    }

    public async Task<Usuario> ModifyUserCredentialsAsync(int id, Usuario usuario, CancellationToken cancellationToken)
    {
        usuario.DNI = _aesGcmCryptoService.Encrypt(usuario.DNI!);
        usuario.NumeroTelefono = _aesGcmCryptoService.Encrypt(usuario.NumeroTelefono!);

        Usuario userUpdated = await _genericRepository.Update(id, usuario);

        await _unitOfWork.Complete(cancellationToken);

        userUpdated.DNI = _aesGcmCryptoService.Decrypt(usuario.DNI!);
        userUpdated.NumeroTelefono = _aesGcmCryptoService.Decrypt(usuario.NumeroTelefono!);
        return userUpdated;
    }

    public async Task<Usuario> DeleteUserWithRelationsAsync(int id)
    {
        var deletedUser = await _unitOfWork.UsuarioRepository.DeleteUserWithRelations(id);

        return deletedUser;
    }

    public async Task<bool> ValidateUserCredentialsToPublishTicket(int userId)
    {
        bool isvalid = true;

        Usuario usuario = await _unitOfWork.UsuarioRepository.GetById(userId);

        if (string.IsNullOrEmpty(usuario.Nombre) ||
        string.IsNullOrEmpty(usuario.DNI) ||
        string.IsNullOrEmpty(usuario.NumeroTelefono) ||
        !usuario.VerificadoEmail ||
        !usuario.VerificadoSms)
        {
            return isvalid = false;
        }

        return isvalid;
    }
}

using GoPass.Application.ServiceFacade;
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

    public async Task<int> GetUserIdFromTokenAsync()
    {
        string authHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString();
        string userId = await CleanTokenAsync(authHeader);

        return int.Parse(userId);
    }
    public async Task<string> CleanTokenAsync(string token)
    {
        string cleanToken = token.StartsWith("Bearer ") ? token.Substring("Bearer ".Length) : token;

        if (string.IsNullOrWhiteSpace(cleanToken))
        {
            throw new Exception("Token nulo o vacío.");
        }

        string decodedToken = await _tokenService.DecodeToken(cleanToken!);

        return decodedToken;
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

    public async Task<Usuario> RegisterUserAsync(Usuario usuario)
    {
        usuario.Password = _passwordHasher.HashPassword(usuario, usuario.Password);

        var nuevoUsuario = await _unitOfWork.UsuarioRepository.Create(usuario);

        if (nuevoUsuario.Id <= 0)
        {
            throw new Exception("El ID del usuario no es válido después de la creación.");
        }

        var userToken = _tokenService.CreateToken(nuevoUsuario);
        nuevoUsuario.Token = userToken;
        await _unitOfWork.UsuarioRepository.StorageToken(usuario.Id, userToken);
        return nuevoUsuario;

    }

    public async Task<Usuario> AuthenticateAsync(string email, string password)
    {
        Usuario userInDb = await _unitOfWork.UsuarioRepository.GetUserByEmail(email);

        PasswordVerificationResult passwordVerification = _passwordHasher.VerifyHashedPassword(userInDb, userInDb.Password, password);

        if (passwordVerification == PasswordVerificationResult.Failed) throw new Exception("Las credenciales no son correctas");

        Usuario user = await _unitOfWork.UsuarioRepository.AuthenticateUser(email, password);

        string token = _tokenService.CreateToken(user);
        user.Token = token;

        return user;
    }

    public async Task<bool> ConfirmResetPasswordAsync(bool reset, string newPassword, string userEmail, CancellationToken cancellationToken)
    {
        try
        {
            var usuario = await _unitOfWork.UsuarioRepository.GetUserByEmail(userEmail);

            if (usuario == null)
            {
                return false;
            }

            usuario.Restablecer = reset;
            usuario.Password = _passwordHasher.HashPassword(usuario, newPassword);

            await _unitOfWork.UsuarioRepository.Update(usuario.Id, usuario);

            await _unitOfWork.Complete(cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

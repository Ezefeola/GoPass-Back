using GoPass.Application.Services.Interfaces;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GoPass.Application.Services.Classes;

public class UsuarioService : GenericService<Usuario>, IUsuarioService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IAesGcmCryptoService _aesGcmCryptoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    public UsuarioService(IUnitOfWork unitOfWork, 
        ITokenService tokenService, 
        IAesGcmCryptoService aesGcmCryptoService, 
        IHttpContextAccessor httpContextAccessor) : base(unitOfWork.UsuarioRepository)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _aesGcmCryptoService = aesGcmCryptoService;
        _httpContextAccessor = httpContextAccessor;
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

    public async Task<Usuario> DeleteUserWithRelationsAsync(int id)
    {
        var deletedUser = await _unitOfWork.UsuarioRepository.DeleteUserWithRelations(id);

        return deletedUser;
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

    public async Task<bool> VerifyEmailExistsAsync(string email)
    {
        bool userEmail = await _unitOfWork.UsuarioRepository.VerifyEmailExists(email);

        return userEmail!;
    }

    public async Task<bool> VerifyDniExistsAsync(string dni, int userId)
    {
        string encriptedDni = _aesGcmCryptoService.Encrypt(dni);
        bool userDni = await _unitOfWork.UsuarioRepository.VerifyDniExists(encriptedDni, userId);

        return userDni;
    }
    public async Task<bool> VerifyPhoneNumberExistsAsync(string phoneNumber, int userId)
    {
        string encriptedPhoneNumber = _aesGcmCryptoService.Encrypt(phoneNumber);
        bool userPhoneNumber = await _unitOfWork.UsuarioRepository.VerifyPhoneNumberExists(encriptedPhoneNumber, userId);

        return userPhoneNumber;
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
        catch (Exception ex)
        {
            return false;
        }
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

using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GoPass.Application.Services.Classes;

public class UserService : GenericService<User>, IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAesGcmCryptoService _aesGcmCryptoService;
    private readonly ITokenService _tokenService;
    private readonly ISmappfter _smappfter;
    private readonly IPasswordHasher<User> _passwordHasher;
    public UserService(IUnitOfWork unitOfWork, 
            IHttpContextAccessor httpContextAccessor, 
            IAesGcmCryptoService aesGcmCryptoService, 
            ITokenService tokenService,
            ISmappfter smappfter
        ) : base(unitOfWork.UserRepository, unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _aesGcmCryptoService = aesGcmCryptoService;
        _tokenService = tokenService;
        _smappfter = smappfter;
        _passwordHasher = new PasswordHasher<User>();    
    }
    public async Task<List<User>> GetAllUsersWithRelationsAsync()
    {
        var users = await _unitOfWork.UserRepository.GetAllUsersWithRelations();

        return users;
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int userId)
    {
        User userInDb = await _unitOfWork.UserRepository.GetById(userId);
        UserResponseDto userInDbResponseDto = _smappfter.Map<User, UserResponseDto>(userInDb);
        return userInDbResponseDto;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _unitOfWork.UserRepository.GetUserByEmail(email);
    }

    public async Task<User> ModifyUserCredentialsAsync(int id, User user, CancellationToken cancellationToken)
    {
        user.DNI = _aesGcmCryptoService.Encrypt(user.DNI!);
        user.PhoneNumber = _aesGcmCryptoService.Encrypt(user.PhoneNumber!);

        User userUpdated = await _genericRepository.Update(id, user);

        await _unitOfWork.Complete(cancellationToken);

        userUpdated.DNI = _aesGcmCryptoService.Decrypt(user.DNI!);
        userUpdated.PhoneNumber = _aesGcmCryptoService.Decrypt(user.PhoneNumber!);
        return userUpdated;
    }

    public async Task<User> DeleteUserWithRelationsAsync(int id)
    {
        var deletedUser = await _unitOfWork.UserRepository.DeleteUserWithRelations(id);

        return deletedUser;
    }

    public async Task<bool> ValidateUserCredentialsToPublishTicket(int userId)
    {
        bool isvalid = true;

        User usuario = await _unitOfWork.UserRepository.GetById(userId);

        if (string.IsNullOrEmpty(usuario.Name) ||
        string.IsNullOrEmpty(usuario.DNI) ||
        string.IsNullOrEmpty(usuario.PhoneNumber) ||
        !usuario.IsEmailVerified ||
        !usuario.IsSmsVerified)
        {
            return isvalid = false;
        }

        return isvalid;
    }
}

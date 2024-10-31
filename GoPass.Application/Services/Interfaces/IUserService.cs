using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface IUserService : IGenericService<User>
{

    Task<List<User>> GetAllUsersWithRelationsAsync();
    Task<User> DeleteUserWithRelationsAsync(int id);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> ModifyUserCredentialsAsync(int id, User usuario, CancellationToken cancellationToken);
    Task<bool> ValidateUserCredentialsToPublishTicket(int userId);
    Task<UserResponseDto> GetUserByIdAsync(int userId);
}

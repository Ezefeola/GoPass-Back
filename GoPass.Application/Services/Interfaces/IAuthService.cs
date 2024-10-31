using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces
{
    public interface IAuthService : IGenericService<User>
    {
        Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto loginRequestDto);
        Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto registerRequestDto, CancellationToken cancellationToken);
        Task<bool> ConfirmResetPasswordAsync(bool isReset, string newPassword, string userEmail, CancellationToken cancellationToken);
        int GetUserIdFromToken();
    }
}
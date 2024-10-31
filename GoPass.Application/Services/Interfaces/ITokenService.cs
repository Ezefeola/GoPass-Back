using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(User usuario);
    string CleanToken(string token);
    string DecodeToken(string token);
}

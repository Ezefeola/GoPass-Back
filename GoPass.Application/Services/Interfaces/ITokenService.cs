using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(Usuario usuario);
    Task<string> DecodeToken(string token);
}

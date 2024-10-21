using GoPass.Application.Services.Interfaces;
using GoPass.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GoPass.Application.Services.Classes;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_configuration["JWT:Key"]!));
    }
    public string CreateToken(Usuario usuario)
    {
        // Verifica si el usuario tiene un ID válido
        if (usuario.Id <= 0)
        {
            throw new Exception("El ID del usuario no es válido.");
        }

        List<Claim> claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
    new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),  // El ID del usuario se almacena aquí
};

        SigningCredentials credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(5),
            SigningCredentials = credentials,
            Issuer = _configuration["JWT:Issuer"],
            Audience = _configuration["JWT:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }


    public async Task<string> DecodeToken(string token)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            // Verificamos si el token es legible
            JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(token);

            // Buscamos el claim "sub" que contiene el ID de usuario
            var userIdClaim = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub");

            if (userIdClaim == null)
            {
                throw new Exception("El claim 'sub' no se encuentra en el token.");
            }

            return userIdClaim.Value;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al decodificar el token: {ex.Message}");
        }
    }

}

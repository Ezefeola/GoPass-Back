using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class AuthMappers
{

    public static Usuario MapToModel(this RegisterRequestDto registerRequestDto)
    {
        return new Usuario
        {
            Email = registerRequestDto.Email,
            Password = registerRequestDto.Password
        };
    }

    public static Usuario MapToModel(this LoginRequestDto loginRequestDto)
    {
        return new Usuario
        {
            Email = loginRequestDto.Email,
            Password = loginRequestDto.Password
        };
    }

    public static LoginResponseDto MapToLoginResponseDto(this Usuario usuario)
    {
        return new LoginResponseDto
        {
            Email = usuario.Email,
            Token = usuario.Token!
        };
    }
}
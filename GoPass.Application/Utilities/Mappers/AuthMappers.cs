using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class AuthMappers
{

    public static User MapToModel(this RegisterRequestDto registerRequestDto)
    {
        return new User
        {
            Email = registerRequestDto.Email,
            Password = registerRequestDto.Password
        };
    }

    public static User MapToModel(this LoginRequestDto loginRequestDto)
    {
        return new User
        {
            Email = loginRequestDto.Email,
            Password = loginRequestDto.Password
        };
    }

    //public static LoginResponseDto MapToLoginResponseDto(this User usuario)
    //{
    //    return new LoginResponseDto
    //    {
    //        Email = usuario.Email,
    //        Token = usuario.Token!
    //    };
    //}
}
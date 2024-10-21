using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class UsuarioMappers
{
    public static Usuario FromRegisterToModel(this RegisterRequestDto registerRequestDto)
    {
        return new Usuario
        {
            Email = registerRequestDto.Email,
            Password = registerRequestDto.Password
        };
    }

    public static Usuario FromLoginToModel(this LoginRequestDto loginRequestDto)
    {
        return new Usuario
        {
            Email = loginRequestDto.Email,
            Password = loginRequestDto.Password
        };
    }

    public static LoginResponseDto FromModelToLoginResponse(this Usuario usuario)
    {
        return new LoginResponseDto
        {
            Email = usuario.Email,
            Token = usuario.Token!
        };
    }

    public static Usuario FromModifyUsuarioRequestToModel(this ModifyUsuarioRequestDto modifyUsuarioRequestDto, Usuario existingData)
    {
        existingData.Nombre = modifyUsuarioRequestDto.Nombre;
        existingData.DNI = modifyUsuarioRequestDto.DNI;
        existingData.NumeroTelefono = modifyUsuarioRequestDto.NumeroTelefono;
        existingData.Image = modifyUsuarioRequestDto.Image;
        existingData.City = modifyUsuarioRequestDto.City;
        existingData.Country = modifyUsuarioRequestDto.Country;
        return existingData;
    }

    public static SellerInformationResponseDto FromModelToSellerInformationResponseDto(this Usuario existingData)
    {
        return new SellerInformationResponseDto
        {
            Id = existingData.Id,
            Nombre = existingData.Nombre!,
            Image = existingData.Image!,
        };
    }
}

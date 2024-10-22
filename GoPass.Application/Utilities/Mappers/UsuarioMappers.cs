using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Response.AuthResponseDTOs;
using GoPass.Domain.DTOs.Response.UsuarioResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class UsuarioMappers
{
    public static Usuario MapToModel(this ModifyUsuarioRequestDto modifyUsuarioRequestDto, Usuario existingData)
    {
        existingData.Nombre = modifyUsuarioRequestDto.Nombre;
        existingData.DNI = modifyUsuarioRequestDto.DNI;
        existingData.NumeroTelefono = modifyUsuarioRequestDto.NumeroTelefono;
        existingData.Image = modifyUsuarioRequestDto.Image;
        existingData.City = modifyUsuarioRequestDto.City;
        existingData.Country = modifyUsuarioRequestDto.Country;
        return existingData;
    }

    public static SellerInformationResponseDto MapToSellerInfoResponseDto(this Usuario usuario)
    {
        return new SellerInformationResponseDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre!,
            Image = usuario.Image!,
        };
    }

    public static ModifyUserDataResponseDto MapToModifyUserDataResponseDto(this Usuario usuario)
    {
        return new ModifyUserDataResponseDto
        {
            City = usuario.City,
            Country = usuario.Country,
            DNI = usuario.DNI,
            Email = usuario.Email,
            Image = usuario.Image,
            Nombre = usuario.Nombre,
            NumeroTelefono = usuario.NumeroTelefono
        };
    }

}

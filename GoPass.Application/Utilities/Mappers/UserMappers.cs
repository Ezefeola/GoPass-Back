using GoPass.Domain.DTOs.Request.AuthRequestDTOs;
using GoPass.Domain.DTOs.Response.UserResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class UserMappers
{
    public static User MapToModel(this ModifyUsuarioRequestDto modifyUsuarioRequestDto, User existingData)
    {
        existingData.Name = modifyUsuarioRequestDto.Name;
        existingData.DNI = modifyUsuarioRequestDto.DNI;
        existingData.PhoneNumber = modifyUsuarioRequestDto.PhoneNumber;
        existingData.Image = modifyUsuarioRequestDto.Image;
        existingData.City = modifyUsuarioRequestDto.City;
        existingData.Country = modifyUsuarioRequestDto.Country;
        return existingData;
    }

    public static SellerInformationResponseDto MapToSellerInfoResponseDto(this User usuario)
    {
        return new SellerInformationResponseDto
        {
            Id = usuario.Id,
            Name = usuario.Name!,
            Image = usuario.Image!,
        };
    }

    public static ModifyUserDataResponseDto MapToModifyUserDataResponseDto(this User usuario)
    {
        return new ModifyUserDataResponseDto
        {
            City = usuario.City,
            Country = usuario.Country,
            DNI = usuario.DNI,
            Email = usuario.Email,
            Image = usuario.Image,
            Name = usuario.Name,
            PhoneNumber = usuario.PhoneNumber
        };
    }

}

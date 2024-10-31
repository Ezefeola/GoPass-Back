using GoPass.Domain.DTOs.Request.AuthRequestDTOs;

namespace GoPass.Application.Utilities.Mappers;

public static class EmailMappers
{
    public static EmailValidationRequestDto AssignEmailValues(this EmailValidationRequestDto emailValidationRequestDto, string usuarioEmail,
        string emailSubject, string emailBody)
    {
        return new EmailValidationRequestDto
        {
            To = usuarioEmail,
            Subject = emailSubject,
            Body = emailBody
        };
    }
}

using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.DTOs.Response;
using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class EntradaMappers
{
    public static Entrada MapToModel(this EntradaRequestDto entradaRequestDto)
    {
        return new Entrada
        {
            CodigoQR = entradaRequestDto.CodigoQR,
            UsuarioId = entradaRequestDto.UsuarioId,
            Verificada = entradaRequestDto.Verificada,
        };
    }

    public static PublishEntradaRequestDto MapToRequestDto(this Entrada entrada)
    {
        return new PublishEntradaRequestDto
        {
            Address = entrada.Address,
            EventDate = entrada.EventDate,
            GameName = entrada.GameName,
            CodigoQR = entrada.CodigoQR,
            Description = entrada.Description,
            Image = entrada.Image,
            Verificada = true
        };
    }

    public static Entrada MapToModel(this PublishEntradaRequestDto publishEntradaRequestDto)
    {
        return new Entrada
        {
            CodigoQR = publishEntradaRequestDto.CodigoQR,
            Verificada = publishEntradaRequestDto.Verificada,
            GameName = publishEntradaRequestDto.GameName,
            Description = publishEntradaRequestDto.Description,
            EventDate = publishEntradaRequestDto.EventDate,
            Address = publishEntradaRequestDto.Address,
            Image = publishEntradaRequestDto.Image
        };
    }

    public static Entrada MapToModel(this PublishEntradaRequestDto publishEntradaRequestDto, Entrada verifiedTicket, int userId)
    {
        return new Entrada
        {
            Address = verifiedTicket.Address,
            EventDate = verifiedTicket.EventDate,
            GameName = verifiedTicket.GameName,
            CodigoQR = verifiedTicket.CodigoQR,
            Description = verifiedTicket.Description,
            Image = verifiedTicket.Image,
            UsuarioId = userId,
            Verificada = true
        };
    }

    public static Reventa MapToModel(this BuyEntradaRequestDto buyEntradaRequestDto)
    {
        return new Reventa
        {
            EntradaId = buyEntradaRequestDto.EntradaId
        };
    }

    public static EntradaResponseDto MapToResponseDto(this Entrada entrada)
    {
        return new EntradaResponseDto
        {
            CodigoQR = entrada.CodigoQR,
            UsuarioId = entrada.UsuarioId,
            Verificada = entrada.Verificada
        };
    }
}

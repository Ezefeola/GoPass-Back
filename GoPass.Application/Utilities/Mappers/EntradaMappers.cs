using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.DTOs.Response;
using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class EntradaMappers
{
    public static Entrada FromEntradaRequestToModel(this EntradaRequestDto entradaRequestDto)
    {
        return new Entrada
        {
            CodigoQR = entradaRequestDto.CodigoQR,
            UsuarioId = entradaRequestDto.UsuarioId,
            Verificada = entradaRequestDto.Verificada,
        };
    }

    public static PublishEntradaRequestDto FromModelToPublishEntradaRequest(this Entrada entrada)
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

    public static Entrada FromPublishEntradaRequestToModel(this PublishEntradaRequestDto publishEntradaRequestDto)
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

    public static Entrada FromEntradaRequestToModel(this PublishEntradaRequestDto publishEntradaRequestDto, Entrada verifiedTicket, int userId)
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

    public static Reventa FromBuyEntradaRequestToModel(this BuyEntradaRequestDto buyEntradaRequestDto)
    {
        return new Reventa
        {
            EntradaId = buyEntradaRequestDto.EntradaId
        };
    }

    public static EntradaResponseDto FromPublishEntradaRequestToResponseDto(this Entrada entrada)
    {
        return new EntradaResponseDto
        {
            CodigoQR = entrada.CodigoQR,
            UsuarioId = entrada.UsuarioId,
            Verificada = entrada.Verificada
        };
    }
}

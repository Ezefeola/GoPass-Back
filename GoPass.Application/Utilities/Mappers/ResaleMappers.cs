using GoPass.Domain.DTOs.Request.ResaleRequestDTOs;
using GoPass.Domain.DTOs.Response.ResaleResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class ResaleMappers
{
    public static Resale MapToModel(this ResaleRequestDto reventaRequestDto)
    {
        return new Resale
        {
            BuyerId = reventaRequestDto.BuyerId,
            ResaleStartDate = reventaRequestDto.ResaleStartDate,
            TicketId = reventaRequestDto.TicketId,    
            Price = reventaRequestDto.Price,
            SellerId = reventaRequestDto.SellerId,
        };
    }

    public static Resale MapToModel(this PublishResaleRequestDto publishReventaRequestDto)
    {
        return new Resale
        {
            ResaleDetail = publishReventaRequestDto.ResaleDetail,
            Price = publishReventaRequestDto.Price,
        };
    }

    public static PublishResaleRequestDto MapToRequestDto(this Resale reventa)
    {
        return new PublishResaleRequestDto
        {
            //EntradaId = reventa.EntradaId,
            Price = reventa.Price,
            ResaleDetail = reventa.ResaleDetail
        };
    }

    public static ResaleResponseDto MapToResponseDto(this Resale reventa)
    {
        return new ResaleResponseDto
        {
            TicketId = reventa.TicketId,
            Price = reventa.Price,
            ResaleDetail = reventa.ResaleDetail,
            BuyerId = reventa.BuyerId,
            ResaleStartDate = reventa.ResaleStartDate,
            SellerId = reventa.SellerId,
        };
    }
}

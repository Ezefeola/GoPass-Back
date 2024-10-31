//using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
//using GoPass.Domain.DTOs.Response.TicketResponseDTOs;
//using GoPass.Domain.Models;

//namespace GoPass.Application.Utilities.Mappers;

//public static class EntradaMappers
//{
//    public static Ticket MapToModel(this TicketRequestDto entradaRequestDto)
//    {
//        return new Ticket
//        {
//            CodigoQR = entradaRequestDto.CodigoQR,
//            UsuarioId = entradaRequestDto.UsuarioId,
//            Verificada = entradaRequestDto.Verificada,
//        };
//    }

//    public static PublishEntradaRequestDto MapToRequestDto(this Ticket entrada)
//    {
//        return new PublishEntradaRequestDto
//        {
//            Address = entrada.Address,
//            EventDate = entrada.EventDate,
//            GameName = entrada.GameName,
//            CodigoQR = entrada.CodigoQR,
//            Description = entrada.Description,
//            Image = entrada.Image,
//            Verificada = true
//        };
//    }

//    public static Ticket MapToModel(this PublishEntradaRequestDto publishEntradaRequestDto)
//    {
//        return new Ticket
//        {
//            CodigoQR = publishEntradaRequestDto.CodigoQR,
//            Verificada = publishEntradaRequestDto.Verificada,
//            GameName = publishEntradaRequestDto.GameName,
//            Description = publishEntradaRequestDto.Description,
//            EventDate = publishEntradaRequestDto.EventDate,
//            Address = publishEntradaRequestDto.Address,
//            Image = publishEntradaRequestDto.Image
//        };
//    }

//    public static Ticket MapToModel(this PublishEntradaRequestDto publishEntradaRequestDto, Ticket verifiedTicket, int userId)
//    {
//        return new Ticket
//        {
//            Address = verifiedTicket.Address,
//            EventDate = verifiedTicket.EventDate,
//            GameName = verifiedTicket.GameName,
//            CodigoQR = verifiedTicket.CodigoQR,
//            Description = verifiedTicket.Description,
//            Image = verifiedTicket.Image,
//            UsuarioId = userId,
//            Verificada = true
//        };
//    }

//    public static Ticket MapToModel(this Ticket entrada, Ticket verifiedTicket, int userId)
//    {
//        return new Ticket
//        {
//            Address = verifiedTicket.Address,
//            EventDate = verifiedTicket.EventDate,
//            GameName = verifiedTicket.GameName,
//            CodigoQR = verifiedTicket.CodigoQR,
//            Description = verifiedTicket.Description,
//            Image = verifiedTicket.Image,
//            UsuarioId = userId,
//            Verificada = true
//        };
//    }

//    public static Resale MapToModel(this BuyEntradaRequestDto buyEntradaRequestDto)
//    {
//        return new Resale
//        {
//            EntradaId = buyEntradaRequestDto.EntradaId
//        };
//    }

//    public static EntradaResponseDto MapToResponseDto(this Ticket entrada)
//    {
//        return new EntradaResponseDto
//        {
//            CodigoQR = entrada.CodigoQR,
//            UsuarioId = entrada.UsuarioId,
//            Verificada = entrada.Verificada
//        };
//    }
//}

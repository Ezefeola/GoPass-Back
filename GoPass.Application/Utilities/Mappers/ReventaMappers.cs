﻿using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.DTOs.Response.ReventaResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class ReventaMappers
{
    public static Reventa MapToModel(this ReventaRequestDto reventaRequestDto)
    {
        return new Reventa
        {
            CompradorId = reventaRequestDto.CompradorId,
            FechaReventa = reventaRequestDto.FechaReventa,
            EntradaId = reventaRequestDto.EntradaId,    
            Precio = reventaRequestDto.Precio,
            VendedorId = reventaRequestDto.VendedorId,
        };
    }

    public static Reventa MapToModel(this PublishReventaRequestDto publishReventaRequestDto)
    {
        return new Reventa
        {
            ResaleDetail = publishReventaRequestDto.ResaleDetail,
            Precio = publishReventaRequestDto.Precio,
        };
    }

    public static PublishReventaRequestDto MapToRequestDto(this Reventa reventa)
    {
        return new PublishReventaRequestDto
        {
            //EntradaId = reventa.EntradaId,
            Precio = reventa.Precio,
            ResaleDetail = reventa.ResaleDetail
        };
    }

    public static ReventaResponseDto MapToResponseDto(this Reventa reventa)
    {
        return new ReventaResponseDto
        {
            EntradaId = reventa.EntradaId,
            Precio = reventa.Precio,
            ResaleDetail = reventa.ResaleDetail,
            CompradorId = reventa.CompradorId,
            FechaReventa = reventa.FechaReventa,
            VendedorId = reventa.VendedorId,
        };
    }
}

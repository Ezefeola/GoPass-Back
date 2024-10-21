using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class HistorialCompraVentaMappers
{
    public static HistorialCompraVenta MapToHistorialCompraVenta(Entrada ticket, Reventa resale, int compradorId)
    {
        return new HistorialCompraVenta
        {
            GameName = ticket.GameName,
            Description = ticket.Description,
            Image = ticket.Image,
            Address = ticket.Address,
            EventDate = ticket.EventDate,
            CodigoQR = ticket.CodigoQR,
            Verificada = ticket.Verificada,
            EntradaId = resale.EntradaId,
            VendedorId = resale.VendedorId,
            CompradorId = compradorId,
            Precio = resale.Precio,
            ResaleDetail = resale.ResaleDetail
        };
    }
}

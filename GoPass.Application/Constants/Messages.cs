namespace GoPass.Application.Constants;

public static class Messages
{
    public const string ERR_QR_NEEDED = "El código QR es obligatorio.";
    public const string ERR_TICKET_UNVERIFIED = "Entrada no verificada.";
    public const string ERR_TICKET_UNFOUND = "Entrada no encontrada.";
    public const string ERR_TICKET_API = "Error en la solicitud HTTP a TicketMaster";
    public const string ERR_UNKNOW = "Error inesperado";
    public const string ERR_TICKET_VERIFY = "Error al verificar Entrada";

    //Si esto llega a pasar sonamos
    public const string ERR_QR_EMPTY = "El código QR es se encuentra vacio en la Base de Datos";
}

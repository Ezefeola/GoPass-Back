namespace GoPass.Application.Services.Interfaces;

public interface ITemplateService
{
    Task<string> ObtenerContenidoTemplateAsync(string nombrePlantilla, Dictionary<string, string> valoresReemplazo);
}
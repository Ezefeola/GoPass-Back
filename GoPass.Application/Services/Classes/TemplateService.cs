using GoPass.Application.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace GoPass.Application.Services.Classes;

public class TemplateService : ITemplateService
{
    private readonly string _rutaBasePlantillas;

    public TemplateService(IWebHostEnvironment environment)
    {

        var directorioRaiz = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
        _rutaBasePlantillas = Path.Combine(directorioRaiz, "GoPass.Infrastructure", "Templates");
    }

    public async Task<string> ObtenerContenidoTemplateAsync(string nombrePlantilla, Dictionary<string, string> valoresReemplazo)
    {

        var rutaPlantilla = Path.Combine(_rutaBasePlantillas, $"{nombrePlantilla}.html");


        if (!File.Exists(rutaPlantilla))
        {
            throw new FileNotFoundException($"No se encontró la plantilla {nombrePlantilla}.");
        }


        string contenidoPlantilla = await File.ReadAllTextAsync(rutaPlantilla);


        foreach (var item in valoresReemplazo)
        {
            contenidoPlantilla = contenidoPlantilla.Replace($"{{{{{item.Key}}}}}", item.Value);
        }

        return contenidoPlantilla;
    }
}

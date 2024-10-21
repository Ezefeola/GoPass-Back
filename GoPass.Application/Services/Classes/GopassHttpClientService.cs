using GoPass.Application.Services.Interfaces;
using GoPass.Domain.Models;
using System.Text.Json;

namespace GoPass.Application.Services.Classes;

public class GopassHttpClientService : IGopassHttpClientService
{
    private readonly HttpClient _httpClient;

    public GopassHttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Entrada> GetTicketByQrAsync(string qrCode)
    {
        var response = await _httpClient.GetAsync($"Faker/get-by-qr/{qrCode}");

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var entrada = JsonSerializer.Deserialize<Entrada>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Esto hace que coincidan propiedades de forma insensible a mayúsculas/minúsculas
        });

        return entrada;
    }
}

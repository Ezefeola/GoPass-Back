using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Response;
using GoPass.Domain.DTOs.Response.TicketFakerResponseDTOs;
using GoPass.Domain.Models;
using System.Text.Json;

namespace GoPass.Application.Services.Classes;

public class GopassHttpClientService : IGopassHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly ICustomAutoMapper customAutoMapper;

    public GopassHttpClientService(HttpClient httpClient, ICustomAutoMapper customAutoMapper)
    {
        _httpClient = httpClient;
        this.customAutoMapper = customAutoMapper;
    }

    public async Task<Entrada> GetTicketByQrAsync(string qrCode)
    {
        var response = await _httpClient.GetAsync($"Faker/get-by-qr/{qrCode}");

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var entrada = JsonSerializer.Deserialize<TicketInFakerResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Esto hace que coincidan propiedades de forma insensible a mayúsculas/minúsculas
        });
        var responseDto = customAutoMapper.Map<TicketInFakerResponseDto, Entrada>(entrada!);
        return responseDto!;
    }
}

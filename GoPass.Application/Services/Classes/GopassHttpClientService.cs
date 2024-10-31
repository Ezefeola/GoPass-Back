using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.ResaleRequestDTOs;
using GoPass.Domain.DTOs.Response.TicketFakerResponseDTOs;
using System.Text.Json;

namespace GoPass.Application.Services.Classes;

public class GopassHttpClientService : IGopassHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly ISmappfter _customAutoMapper;

    public GopassHttpClientService(HttpClient httpClient, ISmappfter customAutoMapper)
    {
        _httpClient = httpClient;
        _customAutoMapper = customAutoMapper;
    }

    public async Task<PublishTicketRequestDto> GetTicketByQrAsync(string qrCode)
    {
        var response = await _httpClient.GetAsync($"Faker/get-by-qr/{qrCode}");

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var entrada = JsonSerializer.Deserialize<TicketInFakerResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true 
        });
        var responseDto = _customAutoMapper.Map<TicketInFakerResponseDto, PublishTicketRequestDto>(entrada!);
        return responseDto!;
    }
}

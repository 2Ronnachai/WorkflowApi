using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WorkflowApi.Application.DTOs.ExternalResponse;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.Infrastructure.ExternalServices
{
    public class PositionApiService(
        HttpClient httpClient,
        ILogger<PositionApiService> logger) : IPositionService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<PositionApiService> _logger = logger;

        public async Task<PositionDto?> GetByIdAsync(
            int id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Fetching position {PositionId}", id);
                
                var response = await _httpClient.GetAsync(
                    $"api/positions/{id}", 
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Failed to get position {PositionId}. Status: {StatusCode}", 
                        id, 
                        response.StatusCode);
                    return null;
                }

                var position = await response.Content.ReadFromJsonAsync<PositionDto>(
                    cancellationToken: cancellationToken);
                
                return position;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching position {PositionId}", id);
                return null; 
            }
        }
    }
}
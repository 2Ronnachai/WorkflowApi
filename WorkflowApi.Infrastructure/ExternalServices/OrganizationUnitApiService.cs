using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WorkflowApi.Application.DTOs.ExternalResponse;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.Infrastructure.ExternalServices
{
    public class OrganizationUnitApiService(
        HttpClient httpClient,
        ILogger<OrganizationUnitApiService> logger) : IOrganizationUnitService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<OrganizationUnitApiService> _logger = logger;

        public async Task<OrganizationUnitDto?> GetByIdAsync(
            int id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Fetching organization unit {OuId}", id);
                
                var response = await _httpClient.GetAsync(
                    $"api/OrganizationChart/{id}", 
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Failed to get OU {OuId}. Status: {StatusCode}", 
                        id, 
                        response.StatusCode);
                    return null;
                }

                var ou = await response.Content.ReadFromJsonAsync<OrganizationUnitDto>(
                    cancellationToken: cancellationToken);
                
                return ou;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching OU {OuId}", id);
                return null;
            }
        }
    }
}
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WorkflowApi.Application.DTOs.ExternalResponse;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.Infrastructure.ExternalServices
{
    public class EmployeeApiService(HttpClient httpClient, 
    ILogger<EmployeeApiService> logger) : IEmployeeService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<EmployeeApiService> _logger = logger;

        public async Task<EmployeeDto?> GetByNIdAsync(string nId, CancellationToken cancellationToken = default)
        {
            try
            {
                 _logger.LogInformation("Fetching employee with NId: {NId}", nId);

                var response = await _httpClient.GetAsync(
                    $"api/employees/nid/{nId}", 
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Failed to get employee {NId}. Status: {StatusCode}", 
                        nId, 
                        response.StatusCode);
                    return null;
                }

                var employee = await response.Content.ReadFromJsonAsync<EmployeeDto>(
                    cancellationToken: cancellationToken);
                
                _logger.LogDebug("Successfully fetched employee {NId}", nId);
                
                return employee;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while fetching employee {NId}", nId);
                throw new InvalidOperationException($"Failed to fetch employee {nId}", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while fetching employee {NId}", nId);
                throw new TimeoutException($"Timeout while fetching employee {nId}", ex);
            }
        }

        public async Task<List<EmployeeDto>> GetByPositionAndOuAsync(
            int positionId, 
            int ouId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching employees for Position: {PositionId}, OU: {OuId}", 
                    positionId, 
                    ouId);
                
                var response = await _httpClient.GetAsync(
                    $"api/employeeassignments/position/{positionId}/organization/{ouId}/employees", 
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var employees = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>(
                    cancellationToken: cancellationToken);
                
                _logger.LogDebug(
                    "Successfully fetched {Count} employees for Position {PositionId}, OU {OuId}", 
                    employees?.Count ?? 0,
                    positionId, 
                    ouId);
                
                return employees ?? [];
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex, 
                    "HTTP error while fetching employees for Position {PositionId}, OU {OuId}", 
                    positionId, 
                    ouId);
                throw new InvalidOperationException(
                    $"Failed to fetch employees for Position {positionId}, OU {ouId}", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(
                    ex, 
                    "Timeout while fetching employees for Position {PositionId}, OU {OuId}", 
                    positionId, 
                    ouId);
                throw new TimeoutException(
                    $"Timeout while fetching employees for Position {positionId}, OU {ouId}", ex);
            }
        }

        public async Task<List<int>> GetEmployeeOuIdsAsync(
            string nId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Fetching OU for employee {NId}", nId);
                
                var response = await _httpClient.GetAsync(
                    $"api/employeeassignments/organization/nid/{nId}", 
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<int>>(
                    cancellationToken: cancellationToken);
                
                var ouId = result ?? [];
                _logger.LogDebug("Successfully fetched OU for employee {NId}", nId);
                
                return ouId;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while fetching OU for employee {NId}", nId);
                throw new InvalidOperationException($"Failed to fetch OU for employee {nId}", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while fetching OU for employee {NId}", nId);
                throw new TimeoutException($"Timeout while fetching OU for employee {nId}", ex);
            }
        }
    }
}
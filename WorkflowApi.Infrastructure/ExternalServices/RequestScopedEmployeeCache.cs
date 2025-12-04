using Microsoft.Extensions.Logging;
using WorkflowApi.Application.DTOs.ExternalResponse;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.Infrastructure.ExternalServices
{
    public class RequestScopedEmployeeCache(
        IEmployeeService innerService,
        ILogger<RequestScopedEmployeeCache> logger) : IEmployeeService
    {
        private readonly IEmployeeService _innerService = innerService;
        private readonly ILogger<RequestScopedEmployeeCache> _logger = logger;
        
        // In-memory cache for this request only
        private readonly Dictionary<string, EmployeeDto> _employeeByNIdCache = [];
        private readonly Dictionary<string, List<int>> _employeeOuCache = [];
        private readonly Dictionary<string, List<EmployeeDto>> _employeesByPositionOuCache = [];

        public async Task<EmployeeDto?> GetByNIdAsync(
            string nId, 
            CancellationToken cancellationToken = default)
        {
            if (_employeeByNIdCache.TryGetValue(nId, out var cached))
            {
                _logger.LogDebug("Request cache HIT for employee {NId}", nId);
                return cached;
            }

            _logger.LogDebug("Request cache MISS for employee {NId}", nId);
            
            var employee = await _innerService.GetByNIdAsync(nId, cancellationToken);
            
            if (employee != null)
            {
                _employeeByNIdCache[nId] = employee;
                _logger.LogDebug("Cached employee {NId} for this request", nId);
            }

            return employee;
        }

        public async Task<List<EmployeeDto>> GetByPositionAndOuAsync(
            int positionId, 
            int ouId, 
            CancellationToken cancellationToken = default)
        {
            var key = $"{positionId}_{ouId}";
            
            if (_employeesByPositionOuCache.TryGetValue(key, out var cached))
            {
                _logger.LogDebug(
                    "Request cache HIT for Position {PositionId}, OU {OuId}", 
                    positionId, 
                    ouId);
                return cached;
            }

            _logger.LogDebug(
                "Request cache MISS for Position {PositionId}, OU {OuId}", 
                positionId, 
                ouId);
            
            var employees = await _innerService.GetByPositionAndOuAsync(
                positionId, 
                ouId, 
                cancellationToken);
            
            _employeesByPositionOuCache[key] = employees;
            _logger.LogDebug(
                "Cached {Count} employees for Position {PositionId}, OU {OuId}", 
                employees.Count,
                positionId, 
                ouId);

            return employees;
        }

        public async Task<List<int>> GetEmployeeOuIdsAsync(
            string nId, 
            CancellationToken cancellationToken = default)
        {
            if (_employeeOuCache.TryGetValue(nId, out var cached))
            {
                _logger.LogDebug("Request cache HIT for employee OU {NId}", nId);
                return cached;
            }

            _logger.LogDebug("Request cache MISS for employee OU {NId}", nId);
            
            var ouIds = await _innerService.GetEmployeeOuIdsAsync(nId, cancellationToken);
            
            _employeeOuCache[nId] = ouIds;
            _logger.LogDebug("Cached {Count} OUs for employee {NId}", ouIds.Count, nId);

            return ouIds;
        }
    }
}
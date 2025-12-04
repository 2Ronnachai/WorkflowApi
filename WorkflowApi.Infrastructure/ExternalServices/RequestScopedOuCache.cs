using Microsoft.Extensions.Logging;
using WorkflowApi.Application.DTOs.ExternalResponse;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.Infrastructure.ExternalServices
{
    public class RequestScopedOuCache(
        IOrganizationUnitService innerService,
        ILogger<RequestScopedOuCache> logger) : IOrganizationUnitService
    {
        private readonly IOrganizationUnitService _innerService = innerService;
        private readonly ILogger<RequestScopedOuCache> _logger = logger;
        private readonly Dictionary<int, OrganizationUnitDto> _cache = new();

        public async Task<OrganizationUnitDto?> GetByIdAsync(
            int id, 
            CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(id, out var cached))
            {
                _logger.LogDebug("Cache HIT for OU {OuId}", id);
                return cached;
            }

            _logger.LogDebug("Cache MISS for OU {OuId}", id);
            
            var ou = await _innerService.GetByIdAsync(id, cancellationToken);
            
            if (ou != null)
            {
                _cache[id] = ou;
            }

            return ou;
        }
    }
}
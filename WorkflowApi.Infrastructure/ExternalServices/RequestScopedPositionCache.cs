using Microsoft.Extensions.Logging;
using WorkflowApi.Application.DTOs.ExternalResponse;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.Infrastructure.ExternalServices
{
    public class RequestScopedPositionCache(
        IPositionService innerService,
        ILogger<RequestScopedPositionCache> logger) : IPositionService
    {
        private readonly IPositionService _innerService = innerService;
        private readonly ILogger<RequestScopedPositionCache> _logger = logger;
        private readonly Dictionary<int, PositionDto> _cache = new();

        public async Task<PositionDto?> GetByIdAsync(
            int id, 
            CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(id, out var cached))
            {
                _logger.LogDebug("Cache HIT for position {PositionId}", id);
                return cached;
            }

            _logger.LogDebug("Cache MISS for position {PositionId}", id);
            
            var position = await _innerService.GetByIdAsync(id, cancellationToken);
            
            if (position != null)
            {
                _cache[id] = position;
            }

            return position;
        }
    }
}
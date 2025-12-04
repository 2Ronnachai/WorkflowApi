using WorkflowApi.Application.DTOs.ExternalResponse;

namespace WorkflowApi.Application.Interfaces
{
    public interface IPositionService
    {
        Task<PositionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
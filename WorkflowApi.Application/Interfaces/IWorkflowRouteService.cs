using WorkflowApi.Application.DTOs.WorkflowRoute;

namespace WorkflowApi.Application.Interfaces
{
    public interface IWorkflowRouteService
    {
        Task<WorkflowRouteResponse> CreateAsync(CreateWorkflowRouteRequest request, CancellationToken cancellationToken = default);
        Task<WorkflowRouteResponse> UpdateAsync(int id, UpdateWorkflowRouteRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<WorkflowRouteResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<WorkflowRouteDetailResponse?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<WorkflowRouteResponse?> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowRouteResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowRouteResponse>> GetActiveRoutesAsync(CancellationToken cancellationToken = default);
    }
}
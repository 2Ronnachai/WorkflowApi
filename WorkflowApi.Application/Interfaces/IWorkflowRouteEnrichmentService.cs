using WorkflowApi.Application.DTOs.WorkflowRoute;

namespace WorkflowApi.Application.Interfaces
{
    public interface IWorkflowRouteEnrichmentService
    {
        Task EnrichRouteDetailAsync(
            WorkflowRouteDetailResponse response, 
            CancellationToken cancellationToken = default);
    }
}
using WorkflowApi.Application.DTOs.WorkflowStep;

namespace WorkflowApi.Application.Interfaces
{
    public interface IWorkflowStepService
    {
        Task<WorkflowStepResponse> CreateAsync(CreateWorkflowStepRequest request, CancellationToken cancellationToken = default);
        Task<WorkflowStepResponse> UpdateAsync(int id, UpdateWorkflowStepRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<WorkflowStepResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<WorkflowStepResponse?> GetWithAssignmentsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowStepResponse>> GetByRouteIdAsync(int routeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowStepResponse>> GetParallelStepsAsync(int routeId, int parallelGroupId, CancellationToken cancellationToken = default);
        Task<WorkflowStepResponse?> GetNextStepAsync(int routeId, int currentSequenceNo, CancellationToken cancellationToken = default);
        Task<WorkflowStepResponse?> GetFinalStepAsync(int routeId, CancellationToken cancellationToken = default);
    }
}
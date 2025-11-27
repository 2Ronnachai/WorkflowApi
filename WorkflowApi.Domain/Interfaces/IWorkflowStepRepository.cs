using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Domain.Interfaces
{
    public interface IWorkflowStepRepository : IRepository<WorkflowStep>
    {
        Task<IEnumerable<WorkflowStep>> GetStepsByRouteIdAsync(int routeId, CancellationToken cancellationToken = default);
        Task<WorkflowStep?> GetStepWithAssignmentsAsync(int stepId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowStep>> GetParallelStepsAsync(int routeId, int parallelGroupId, CancellationToken cancellationToken = default);
        Task<WorkflowStep?> GetNextStepAsync(int routeId, int currentSequenceNo, CancellationToken cancellationToken = default);
        Task<WorkflowStep?> GetFinalStepAsync(int routeId, CancellationToken cancellationToken = default);
        Task<bool> IsSequenceNoExistsAsync(int routeId, int sequenceNo, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}

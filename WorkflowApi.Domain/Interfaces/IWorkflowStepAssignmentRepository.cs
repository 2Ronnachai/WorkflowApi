using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Domain.Interfaces
{
    public interface IWorkflowStepAssignmentRepository : IRepository<WorkflowStepAssignment>
    {
        Task<IEnumerable<WorkflowStepAssignment>> GetAssignmentsByStepIdAsync(int stepId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowStepAssignment>> GetPositionBasedAssignmentsAsync(int stepId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowStepAssignment>> GetEmployeeBasedAssignmentsAsync(int stepId, CancellationToken cancellationToken = default);
        Task<WorkflowStepAssignment?> GetAssignmentByNIdAsync(int stepId, string nId, CancellationToken cancellationToken = default);
    }
}

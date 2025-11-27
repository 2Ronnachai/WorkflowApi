using WorkflowApi.Application.DTOs.WorkflowStepAssignment;

namespace WorkflowApi.Application.Interfaces
{
    public interface IWorkflowStepAssignmentService
    {
        Task<WorkflowStepAssignmentResponse> CreateAsync(CreateWorkflowStepAssignmentRequest request, CancellationToken cancellationToken = default);
        Task<WorkflowStepAssignmentResponse> UpdateAsync(int id, UpdateWorkflowStepAssignmentRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<WorkflowStepAssignmentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowStepAssignmentResponse>> GetByStepIdAsync(int stepId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowStepAssignmentResponse>> GetPositionBasedAssignmentsAsync(int stepId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowStepAssignmentResponse>> GetEmployeeBasedAssignmentsAsync(int stepId, CancellationToken cancellationToken = default);
        Task<WorkflowStepAssignmentResponse?> GetByNIdAsync(int stepId, string nId, CancellationToken cancellationToken = default);
    }
}
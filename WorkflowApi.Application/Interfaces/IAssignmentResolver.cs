using WorkflowApi.Application.DTOs.Resolution;
using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Application.Interfaces
{
    public interface IAssignmentResolver
    {
        Task<List<ResolvedAssigneeResponse>> ResolveAsync(
            WorkflowStepAssignment assignment,
            ResolveWorkflowRequest request,
            CancellationToken cancellationToken = default
        );
    }
}
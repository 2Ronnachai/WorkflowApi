using WorkflowApi.Application.DTOs.Condition;
using WorkflowApi.Application.DTOs.Resolution;

namespace WorkflowApi.Application.Interfaces
{
    public interface IWorkflowResolutionService
    {
        Task<ResolvedWorkflowResponse> ResolveWorkflowAsync(
            ResolveWorkflowRequest request, 
            CancellationToken cancellationToken = default);
        
        Task<EvaluateConditionResponse> EvaluateConditionAsync(
            EvaluateConditionRequest request, 
            CancellationToken cancellationToken = default);
    }
}
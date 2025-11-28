using WorkflowApi.Application.DTOs.Condition;

namespace WorkflowApi.Application.Interfaces
{
    public interface IDelegationResolver
    {
        Task<string> ResolveAsync(
            string nId,
            int routeId,
            string documentType,
            int stepId,
            CancellationToken cancellationToken = default);

        Task<DelegationResult> ResolveWithDetailsAsync(
            string nId,
            int routeId,
            string documentType,
            int stepId,
            CancellationToken cancellationToken = default);
    }
}
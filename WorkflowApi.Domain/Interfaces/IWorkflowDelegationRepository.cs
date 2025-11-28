using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Domain.Interfaces
{
    public interface IWorkflowDelegationRepository : IRepository<WorkflowDelegation>
    {
        Task<WorkflowDelegation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        
        Task<WorkflowDelegation?> GetActiveDelegationAsync(
            string delegator,
            DateTime currentDate,
            CancellationToken cancellationToken = default);
        
        Task<WorkflowDelegation?> GetActiveDelegationForRouteAsync(
            string delegator,
            int routeId,
            DateTime currentDate,
            CancellationToken cancellationToken = default);
        
        Task<WorkflowDelegation?> GetActiveDelegationForDocumentTypeAsync(
            string delegator,
            string documentType,
            DateTime currentDate,
            CancellationToken cancellationToken = default);
        
        Task<WorkflowDelegation?> GetActiveDelegationForStepAsync(
            string delegator,
            int stepId,
            DateTime currentDate,
            CancellationToken cancellationToken = default);
        
        Task<List<WorkflowDelegation>> GetByDelegatorAsync(
            string delegator,
            CancellationToken cancellationToken = default);
        
        Task<List<WorkflowDelegation>> GetByDelegateAsync(
            string delegatee,
            CancellationToken cancellationToken = default);
    }
}
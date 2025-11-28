using WorkflowApi.Application.DTOs.WorkflowDelegation;

namespace WorkflowApi.Application.Interfaces
{
    public interface IWorkflowDelegationService
    {
        Task<DelegationResponse> CreateSimpleAsync(
            SimpleDelegationRequest request,
            CancellationToken cancellationToken = default);

        Task<DelegationResponse> CreateAsync(
            CreateDelegationRequest request,
            CancellationToken cancellationToken = default);

        Task<DelegationResponse> UpdateAsync(
            int id,
            UpdateDelegationRequest request,
            CancellationToken cancellationToken = default);

        Task<DelegationResponse?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<DelegationResponse>> GetByDelegatorAsync(
            string delegatorNId,
            CancellationToken cancellationToken = default);

        Task<List<DelegationResponse>> GetByDelegateAsync(
            string delegateNId,
            CancellationToken cancellationToken = default);

        Task<string?> GetActiveDelegateAsync(
            string delegatorNId,
            int? routeId = null,
            string? documentType = null,
            int? stepId = null,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
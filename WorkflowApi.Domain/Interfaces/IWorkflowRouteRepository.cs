using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Domain.Interfaces
{
    public interface IWorkflowRouteRepository : IRepository<WorkflowRoute>
    {
        Task<WorkflowRoute?> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default);
        Task<WorkflowRoute?> GetWithStepsAsync(int id, CancellationToken cancellationToken = default);
        Task<WorkflowRoute?> GetCompleteRouteAsync(int id, CancellationToken cancellationToken = default);
        Task<WorkflowRoute?> GetCompleteRouteByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowRoute>> GetActiveRoutesAsync(CancellationToken cancellationToken = default);
        Task<bool> IsRouteNameExistsAsync(string routeName, string documentType, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}

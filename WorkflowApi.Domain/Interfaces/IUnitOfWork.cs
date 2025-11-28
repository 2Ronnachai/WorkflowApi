namespace WorkflowApi.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IWorkflowRouteRepository WorkflowRoutes { get; }
        IWorkflowStepRepository WorkflowSteps { get; }
        IWorkflowStepAssignmentRepository WorkflowStepAssignments { get; }
        IWorkflowDelegationRepository WorkflowDelegations { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
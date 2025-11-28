using Microsoft.EntityFrameworkCore.Storage;
using WorkflowApi.Domain.Interfaces;
using WorkflowApi.Infrastructure.Data;

namespace WorkflowApi.Infrastructure.Repositories
{
    public class UnitOfWork(WorkflowApiDbContext context) : IUnitOfWork
    {
        private readonly WorkflowApiDbContext _context = context;
        private IDbContextTransaction? _transaction;

        // Lazy initialization of repositories
        private IWorkflowRouteRepository? _workflowRouteRepository;
        private IWorkflowStepRepository? _workflowStepRepository;
        private IWorkflowStepAssignmentRepository? _workflowStepAssignmentRepository;
        private IWorkflowDelegationRepository? _workflowDelegationRepository;

        // Repository properties
        public IWorkflowRouteRepository WorkflowRoutes =>
            _workflowRouteRepository ??= new WorkflowRouteRepository(_context);
        public IWorkflowStepRepository WorkflowSteps =>
            _workflowStepRepository ??= new WorkflowStepRepository(_context);
        public IWorkflowStepAssignmentRepository WorkflowStepAssignments =>
            _workflowStepAssignmentRepository ??= new WorkflowStepAssignmentRepository(_context);
        public IWorkflowDelegationRepository WorkflowDelegations =>
            _workflowDelegationRepository ??= new WorkflowDelegationRepository(_context);

        // Save changes to the database
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        // Begin a new transaction
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        // Commit the current transaction
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        // Rollback the current transaction
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // Dispose the DbContext and transaction
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
        }
    }
}
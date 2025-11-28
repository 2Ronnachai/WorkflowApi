using Microsoft.EntityFrameworkCore;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Enums;
using WorkflowApi.Domain.Interfaces;
using WorkflowApi.Infrastructure.Data;

namespace WorkflowApi.Infrastructure.Repositories
{
    public class WorkflowDelegationRepository(WorkflowApiDbContext context) : Repository<WorkflowDelegation>(context), IWorkflowDelegationRepository
    {
        public async Task<WorkflowDelegation?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Route)
                .Include(d => d.Step)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<WorkflowDelegation?> GetActiveDelegationAsync(
            string delegator,
            DateTime currentDate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d =>
                    d.Delegator == delegator &&
                    d.IsActive &&
                    d.StartDate <= currentDate &&
                    d.EndDate >= currentDate &&
                    d.Scope == DelegationScope.All)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<WorkflowDelegation?> GetActiveDelegationForRouteAsync(
            string delegator,
            int routeId,
            DateTime currentDate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d =>
                    d.Delegator == delegator &&
                    d.IsActive &&
                    d.StartDate <= currentDate &&
                    d.EndDate >= currentDate &&
                    d.Scope == DelegationScope.SpecificRoute &&
                    d.RouteId == routeId)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<WorkflowDelegation?> GetActiveDelegationForDocumentTypeAsync(
            string delegator,
            string documentType,
            DateTime currentDate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d =>
                    d.Delegator == delegator &&
                    d.IsActive &&
                    d.StartDate <= currentDate &&
                    d.EndDate >= currentDate &&
                    d.Scope == DelegationScope.SpecificDocType &&
                    d.DocumentType == documentType)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<WorkflowDelegation?> GetActiveDelegationForStepAsync(
            string delegator,
            int stepId,
            DateTime currentDate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d =>
                    d.Delegator == delegator &&
                    d.IsActive &&
                    d.StartDate <= currentDate &&
                    d.EndDate >= currentDate &&
                    d.Scope == DelegationScope.SpecificStep &&
                    d.StepId == stepId)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<WorkflowDelegation>> GetByDelegatorAsync(
            string delegator,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Route)
                .Include(d => d.Step)
                .Where(d => d.Delegator == delegator)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<WorkflowDelegation>> GetByDelegateAsync(
            string delegatee,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Route)
                .Include(d => d.Step)
                .Where(d => d.Delegatee == delegatee)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
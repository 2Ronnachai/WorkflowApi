using Microsoft.EntityFrameworkCore;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Interfaces;
using WorkflowApi.Infrastructure.Data;

namespace WorkflowApi.Infrastructure.Repositories
{
    public class WorkflowRouteRepository(WorkflowApiDbContext context) 
        : Repository<WorkflowRoute>(context), IWorkflowRouteRepository
    {
        public async Task<WorkflowRoute?> GetByDocumentTypeAsync(
            string documentType, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(r => r.DocumentType == documentType && r.IsActive)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<WorkflowRoute?> GetWithStepsAsync(
            int id, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(r => r.Steps.OrderBy(s => s.SequenceNo))
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<WorkflowRoute?> GetCompleteRouteAsync(
            int id, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(r => r.Steps.OrderBy(s => s.SequenceNo))
                    .ThenInclude(s => s.Assignments)
                .Include(r => r.Steps)
                    .ThenInclude(s => s.ReturnStep)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<WorkflowRoute?> GetCompleteRouteByDocumentTypeAsync(
            string documentType, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(r => r.Steps.OrderBy(s => s.SequenceNo))
                    .ThenInclude(s => s.Assignments)
                .Include(r => r.Steps)
                    .ThenInclude(s => s.ReturnStep)
                .FirstOrDefaultAsync(r => r.DocumentType == documentType && r.IsActive, cancellationToken);
        }

        public async Task<IEnumerable<WorkflowRoute>> GetActiveRoutesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(r => r.IsActive)
                .OrderBy(r => r.RouteName)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsRouteNameExistsAsync(
            string routeName, 
            string documentType, 
            int? excludeId = null, 
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(r => 
                r.RouteName == routeName && 
                r.DocumentType == documentType);

            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
    }
}

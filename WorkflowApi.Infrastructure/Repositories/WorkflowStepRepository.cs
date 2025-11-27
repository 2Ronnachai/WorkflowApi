using Microsoft.EntityFrameworkCore;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Interfaces;
using WorkflowApi.Infrastructure.Data;

namespace WorkflowApi.Infrastructure.Repositories
{
    public class WorkflowStepRepository(WorkflowApiDbContext context) 
        : Repository<WorkflowStep>(context), IWorkflowStepRepository
    {
        public async Task<IEnumerable<WorkflowStep>> GetStepsByRouteIdAsync(
            int routeId, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.RouteId == routeId)
                .OrderBy(s => s.SequenceNo)
                .ToListAsync(cancellationToken);
        }

        public async Task<WorkflowStep?> GetStepWithAssignmentsAsync(
            int stepId, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.Assignments)
                .FirstOrDefaultAsync(s => s.Id == stepId, cancellationToken);
        }

        public async Task<IEnumerable<WorkflowStep>> GetParallelStepsAsync(
            int routeId, 
            int parallelGroupId, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.RouteId == routeId && s.ParallelGroupId == parallelGroupId)
                .OrderBy(s => s.SequenceNo)
                .ToListAsync(cancellationToken);
        }

        public async Task<WorkflowStep?> GetNextStepAsync(
            int routeId, 
            int currentSequenceNo, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.RouteId == routeId && s.SequenceNo > currentSequenceNo)
                .OrderBy(s => s.SequenceNo)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<WorkflowStep?> GetFinalStepAsync(
            int routeId, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.RouteId == routeId && s.IsFinalStep)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> IsSequenceNoExistsAsync(
            int routeId, 
            int sequenceNo, 
            int? excludeId = null, 
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(s => 
                s.RouteId == routeId && 
                s.SequenceNo == sequenceNo);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
    }
}

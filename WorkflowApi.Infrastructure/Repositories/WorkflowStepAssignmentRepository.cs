using Microsoft.EntityFrameworkCore;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Enums;
using WorkflowApi.Domain.Interfaces;
using WorkflowApi.Infrastructure.Data;

namespace WorkflowApi.Infrastructure.Repositories
{
    public class WorkflowStepAssignmentRepository(WorkflowApiDbContext context) 
        : Repository<WorkflowStepAssignment>(context), IWorkflowStepAssignmentRepository
    {
        public async Task<IEnumerable<WorkflowStepAssignment>> GetAssignmentsByStepIdAsync(
            int stepId, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.StepId == stepId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<WorkflowStepAssignment>> GetPositionBasedAssignmentsAsync(
            int stepId, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.StepId == stepId && a.AssignmentType == AssignmentType.Position)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<WorkflowStepAssignment>> GetEmployeeBasedAssignmentsAsync(
            int stepId, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.StepId == stepId && a.AssignmentType == AssignmentType.SpecificEmployee)
                .ToListAsync(cancellationToken);
        }

        public async Task<WorkflowStepAssignment?> GetAssignmentByNIdAsync(
            int stepId, 
            string nId, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(
                    a => a.StepId == stepId && a.NId == nId, 
                    cancellationToken);
        }
    }
}

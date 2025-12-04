using Microsoft.Extensions.Logging;
using WorkflowApi.Application.DTOs.WorkflowRoute;
using WorkflowApi.Application.DTOs.WorkflowStep;
using WorkflowApi.Application.DTOs.WorkflowStepAssignment;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.Application.Services
{
    public class WorkflowRouteEnrichmentService(
        IEmployeeService employeeService,
        IPositionService positionService,
        IOrganizationUnitService ouService,
        ILogger<WorkflowRouteEnrichmentService> logger) : IWorkflowRouteEnrichmentService
    {
        private readonly IEmployeeService _employeeService = employeeService;
        private readonly IPositionService _positionService = positionService;
        private readonly IOrganizationUnitService _ouService = ouService;
        private readonly ILogger<WorkflowRouteEnrichmentService> _logger = logger;

        public async Task EnrichRouteDetailAsync(
            WorkflowRouteDetailResponse response, 
            CancellationToken cancellationToken = default)
        {
            if (response.Steps == null || response.Steps.Count == 0)
                return;

            foreach (var step in response.Steps)
            {
                await EnrichStepAsync(step, cancellationToken);
            }
        }

        private async Task EnrichStepAsync(
            WorkflowStepResponse step, 
            CancellationToken cancellationToken)
        {
            if (step.Assignments == null || step.Assignments.Count == 0)
                return;

            var enrichmentTasks = step.Assignments.Select(assignment => 
                EnrichAssignmentAsync(assignment, cancellationToken));

            await Task.WhenAll(enrichmentTasks);
        }

        private async Task EnrichAssignmentAsync(
            WorkflowStepAssignmentResponse assignment, 
            CancellationToken cancellationToken)
        {
            try
            {
                var tasks = new List<Task>();

                // Enrich Position Name
                if (assignment.PositionId.HasValue && assignment.PositionId.Value > 0)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var position = await _positionService.GetByIdAsync(
                            assignment.PositionId.Value,
                            cancellationToken);

                        assignment.PositionName = position?.Title;
                    }, cancellationToken));
                }

                // Enrich OU Name
                if (assignment.OrganizationalUnitId.HasValue && assignment.OrganizationalUnitId.Value > 0)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var ou = await _ouService.GetByIdAsync(
                            assignment.OrganizationalUnitId.Value,
                            cancellationToken);

                        assignment.OrganizationalUnitName = ou?.Name;
                    }, cancellationToken));
                }

                // Enrich Employee Name
                if (!string.IsNullOrWhiteSpace(assignment.NId))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var employee = await _employeeService.GetByNIdAsync(
                            assignment.NId,
                            cancellationToken);

                        assignment.EmployeeName = employee?.FullName;
                    }, cancellationToken));
                }

                if(tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex, 
                    "Failed to enrich assignment {AssignmentId}", 
                    assignment.Id);
            }
        }
    }
}
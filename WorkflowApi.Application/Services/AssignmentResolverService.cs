using WorkflowApi.Application.DTOs.Resolution;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Application.Services
{
    public class AssignmentResolverService(
        IDelegationResolver delegationResolver,
        IEmployeeService employeeService) : IAssignmentResolver
    {
        private readonly IDelegationResolver _delegationResolver = delegationResolver;
        private readonly IEmployeeService _employeeService = employeeService;

        public async Task<List<ResolvedAssigneeResponse>> ResolveAsync(
            WorkflowStepAssignment assignment,
            ResolveWorkflowRequest request,
            CancellationToken cancellationToken = default)
        {
            return assignment.AssignmentType switch
            {
                AssignmentType.SpecificEmployee => await ResolveSpecificEmployeeAsync(
                    assignment, request, cancellationToken),

                AssignmentType.Position => await ResolvePositionAsync(
                    assignment, request, cancellationToken),

                _ => throw new InvalidOperationException(
                    $"Unsupported AssignmentType: {assignment.AssignmentType}")
            };
        }

        private async Task<List<ResolvedAssigneeResponse>> ResolveSpecificEmployeeAsync(
            WorkflowStepAssignment assignment,
            ResolveWorkflowRequest request,
            CancellationToken cancellationToken)
        {
            var originalNId = assignment.NId!;

            // Resolve delegation
            var delegationResult = await _delegationResolver.ResolveWithDetailsAsync(
                originalNId,
                request.RouteId,
                request.DocumentType,
                assignment.StepId,
                cancellationToken);

            // Get employee details
            var employee = await _employeeService.GetByNIdAsync(
                delegationResult.FinalNId, 
                cancellationToken);

            var assignee = new ResolvedAssigneeResponse
            {
                AssignmentId = assignment.Id,
                AssignmentType = assignment.AssignmentType.ToString(),
                NId = delegationResult.FinalNId,
                EmployeeName = employee?.FullName ?? delegationResult.FinalNId,
                Email = employee?.Email ?? string.Empty,
                Delegator = delegationResult.IsDelegated ? originalNId : null,
                IsDelegated = delegationResult.IsDelegated,
                DelegationReason = delegationResult.DelegationReason,
                ResolutionMethod = ResolutionConstants.DirectAssignment
            };

            return [assignee];
        }

        private async Task<List<ResolvedAssigneeResponse>> ResolvePositionAsync(
            WorkflowStepAssignment assignment,
            ResolveWorkflowRequest request,
            CancellationToken cancellationToken)
        {
            var assignees = new List<ResolvedAssigneeResponse>();

            // 1. Resolve organizational units
            var targetOuIds = ResolveOrganizationalUnit(assignment, request);

            // 2. For each OU, find employees
            foreach (var ouId in targetOuIds)
            {
                var employees = await _employeeService.GetByPositionAndOuAsync(
                    assignment.PositionId!.Value,
                    ouId,
                    cancellationToken);

                // 3. For each employee, resolve delegation
                foreach (var emp in employees)
                {
                    var delegationResult = await _delegationResolver.ResolveWithDetailsAsync(
                        emp.NId,
                        request.RouteId,
                        request.DocumentType,
                        assignment.StepId,
                        cancellationToken);

                    var employeeName = emp.FullName;
                    var email = emp.Email;

                    if (delegationResult.IsDelegated)
                    {
                        var delegatedEmp = await _employeeService.GetByNIdAsync(
                            delegationResult.FinalNId,
                            cancellationToken);

                        if (delegatedEmp != null)
                        {
                            employeeName = delegatedEmp.FullName;
                            email = delegatedEmp.Email;
                        }
                    }

                    assignees.Add(new ResolvedAssigneeResponse
                    {
                        AssignmentId = assignment.Id,
                        AssignmentType = assignment.AssignmentType.ToString(),
                        PositionId = assignment.PositionId,
                        OrganizationalUnitId = ouId,
                        NId = delegationResult.FinalNId,
                        EmployeeName = employeeName,
                        Email = email,
                        Delegator = delegationResult.IsDelegated ? emp.NId : null,
                        IsDelegated = delegationResult.IsDelegated,
                        DelegationReason = delegationResult.DelegationReason,
                        ResolutionMethod = GetResolutionMethod(assignment.OUResolutionMode)
                    });
                }
            }

            return assignees;
        }

        private List<int> ResolveOrganizationalUnit(
            WorkflowStepAssignment assignment,
            ResolveWorkflowRequest request)
        {
            return assignment.OUResolutionMode switch
            {
                OUResolutionMode.FollowOrigin => request.OrganizationalUnitIds,

                OUResolutionMode.FollowInitiator =>
                    GetInitiatorOuIds(request.CreatedBy),

                OUResolutionMode.Fixed =>
                    [assignment.OrganizationalUnitId!.Value],

                _ => throw new InvalidOperationException(
                    $"Unsupported OUResolutionMode: {assignment.OUResolutionMode}")
            };
        }

        private List<int> GetInitiatorOuIds(string createdBy)
        {
            var employeeOuIds = _employeeService.GetEmployeeOuIdsAsync(createdBy).Result;
            return employeeOuIds;
        }

        private static string GetResolutionMethod(OUResolutionMode mode)
        {
            return mode switch
            {
                OUResolutionMode.FollowOrigin => nameof(OUResolutionMode.FollowOrigin),
                OUResolutionMode.FollowInitiator => nameof(OUResolutionMode.FollowInitiator),
                OUResolutionMode.Fixed => nameof(OUResolutionMode.Fixed),
                _ => "Unknown"
            };
        }
    }
}
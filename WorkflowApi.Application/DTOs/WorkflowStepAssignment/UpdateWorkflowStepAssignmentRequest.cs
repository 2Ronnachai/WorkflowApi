using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Application.DTOs.WorkflowStepAssignment
{
    public class UpdateWorkflowStepAssignmentRequest
    {
        public AssignmentType AssignmentType { get; set; }
        public int? PositionId { get; set; }
        public OUResolutionMode OUResolutionMode { get; set; }
        public int? OrganizationalUnitId { get; set; }
        public bool AllowFallback { get; set; }
        public string? NId { get; set; }
    }
}
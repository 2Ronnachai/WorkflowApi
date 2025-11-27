using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Application.DTOs.WorkflowStepAssignment
{
    public class CreateWorkflowStepAssignmentRequest
    {
        public int StepId { get; set; }
        public AssignmentType AssignmentType { get; set; }
        
        // Position-based
        public int? PositionId { get; set; }
        public OUResolutionMode OUResolutionMode { get; set; } = OUResolutionMode.FollowOrigin;
        public int? OrganizationalUnitId { get; set; }
        public bool AllowFallback { get; set; } = true;
        
        // Employee-based
        public string? NId { get; set; }
    }
}
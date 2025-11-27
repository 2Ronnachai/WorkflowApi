namespace WorkflowApi.Application.DTOs.WorkflowStepAssignment
{
    public class WorkflowStepAssignmentResponse
    {
        public int Id { get; set; }
        public int StepId { get; set; }
        public string AssignmentType { get; set; } = string.Empty; // "Position" | "SpecificEmployee"
        
        // Position-based
        public int? PositionId { get; set; }
        public string? PositionName { get; set; } // From cache/external API
        public string OUResolutionMode { get; set; } = string.Empty; // "Fixed" | "FollowOrigin" | "FollowInitiator"
        public int? OrganizationalUnitId { get; set; }
        public string? OrganizationalUnitName { get; set; } // From cache
        
        // Employee-based
        public string? NId { get; set; }
        public string? EmployeeName { get; set; } // Optional: from external API
    }
}
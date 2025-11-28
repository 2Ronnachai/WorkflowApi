namespace WorkflowApi.Application.DTOs.Resolution
{
    public class ResolvedAssigneeResponse
    {
        public int AssignmentId { get; set; }
        public string AssignmentType { get; set; } = string.Empty;
        
        // Resolved Employee Info
        public string NId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // Resolution Context
        public int? PositionId { get; set; }
        public string? PositionName { get; set; }
        public int? OrganizationalUnitId { get; set; }
        public string? OrganizationalUnitName { get; set; }

        // Delegation Details
        public string? Delegator { get; set; }
        public bool IsDelegated { get; set; }
        public string? DelegationReason { get; set; }
        
        // Resolution Details
        public string ResolutionMethod { get; set; } = string.Empty; 
        // "DirectAssignment" | "PositionInOriginOU" | "PositionInInitiatorOU" | "PositionInFixedOU" | "FallbackToParentOU"
    }
}
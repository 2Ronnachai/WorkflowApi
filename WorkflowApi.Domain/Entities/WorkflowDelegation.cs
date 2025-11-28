using WorkflowApi.Domain.Common;
using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Domain.Entities
{
    public class WorkflowDelegation : AuditableEntity
    {
        public string Delegator { get; set; } = string.Empty;
        public string Delegatee { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DelegationScope Scope { get; set; } = DelegationScope.All;

        public int? RouteId { get; set; }
        public WorkflowRoute? Route { get; set; }

        public string? DocumentType { get; set; }
        
        public int? StepId { get; set; }
        public WorkflowStep? Step { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Reason { get; set; }

        // Validation
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Delegator))
                throw new InvalidOperationException("Delegator is required");
            
            if (string.IsNullOrWhiteSpace(Delegatee))
                throw new InvalidOperationException("Delegatee is required");
            
            if (Delegator == Delegatee)
                throw new InvalidOperationException("Cannot delegate to yourself");
            
            if (StartDate >= EndDate)
                throw new InvalidOperationException("StartDate must be before EndDate");
            
            if (Scope == DelegationScope.SpecificRoute && RouteId == null)
                throw new InvalidOperationException("RouteId is required when Scope is SpecificRoute");
            
            if (Scope == DelegationScope.SpecificDocType && string.IsNullOrWhiteSpace(DocumentType))
                throw new InvalidOperationException("DocumentType is required when Scope is SpecificDocType");
            
            if (Scope == DelegationScope.SpecificStep && StepId == null)
                throw new InvalidOperationException("StepId is required when Scope is SpecificStep");
        }
    }
}
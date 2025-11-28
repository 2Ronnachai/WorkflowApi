using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Application.DTOs.WorkflowDelegation
{
    public class CreateDelegationRequest
    {
        public string Delegator { get; set; } = string.Empty;
        public string Delegatee { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DelegationScope Scope { get; set; } = DelegationScope.All;
        public int? RouteId { get; set; }
        public string? DocumentType { get; set; }
        public int? StepId { get; set; }
        public string? Reason { get; set; }
    }
}
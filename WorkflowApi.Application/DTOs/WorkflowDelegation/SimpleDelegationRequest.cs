namespace WorkflowApi.Application.DTOs.WorkflowDelegation
{
    public class SimpleDelegationRequest
    {
        public string Delegator { get; set; } = string.Empty;
        public string Delegatee { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }
}
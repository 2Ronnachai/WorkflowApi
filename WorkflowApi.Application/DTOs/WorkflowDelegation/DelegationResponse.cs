namespace WorkflowApi.Application.DTOs.WorkflowDelegation
{
    public class DelegationResponse
    {
        public int Id { get; set; }
        public string DelegatorNId { get; set; } = string.Empty;
        public string DelegateNId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Scope { get; set; } = string.Empty;
        public int? RouteId { get; set; }
        public string? RouteName { get; set; }
        public string? DocumentType { get; set; }
        public int? StepId { get; set; }
        public string? StepName { get; set; }
        public string? Reason { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
namespace WorkflowApi.Application.DTOs.Condition
{
    public class DelegationResult
    {
        public string FinalNId { get; set; } = string.Empty;
        public string OriginalNId { get; set; } = string.Empty;
        public bool IsDelegated { get; set; }
        public string? DelegationReason { get; set; }
        public List<string> DelegationChain { get; set; } = [];
    }
}
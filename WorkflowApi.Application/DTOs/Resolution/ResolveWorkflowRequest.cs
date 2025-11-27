namespace WorkflowApi.Application.DTOs.Resolution
{
    public class ResolveWorkflowRequest
    {
        public string DocumentType { get; set; } = string.Empty;
        public List<int> OrganizationalUnitIds { get; set; } = [];
        public List<string>? OrganizationalUnitCodes { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public Dictionary<string, object>? ConditionalData { get; set; }
    }
}
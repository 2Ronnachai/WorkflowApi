namespace WorkflowApi.Application.DTOs.Resolution
{
    public class ResolvedWorkflowResponse
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public List<ResolvedWorkflowStepResponse> Steps { get; set; } = new();
    }
}
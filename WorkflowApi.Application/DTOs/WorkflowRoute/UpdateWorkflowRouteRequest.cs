namespace WorkflowApi.Application.DTOs.WorkflowRoute
{
    public class UpdateWorkflowRouteRequest
    {
        public string RouteName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
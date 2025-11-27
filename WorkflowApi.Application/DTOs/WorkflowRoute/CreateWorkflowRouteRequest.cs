using WorkflowApi.Application.DTOs.WorkflowStep;

namespace WorkflowApi.Application.DTOs.WorkflowRoute
{
    public class CreateWorkflowRouteRequest
    {
        public string RouteName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public List<CreateWorkflowStepRequest> Steps { get; set; } = [];
    }
}
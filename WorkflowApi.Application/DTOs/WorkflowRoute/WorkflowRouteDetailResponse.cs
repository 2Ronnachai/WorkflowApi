using WorkflowApi.Application.DTOs.WorkflowStep;

namespace WorkflowApi.Application.DTOs.WorkflowRoute
{
    public class WorkflowRouteDetailResponse : WorkflowRouteResponse
    {
        public List<WorkflowStepResponse> Steps { get; set; } = [];
    }
}
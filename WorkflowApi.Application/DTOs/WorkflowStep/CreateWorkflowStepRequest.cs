using WorkflowApi.Application.DTOs.WorkflowStepAssignment;
using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Application.DTOs.WorkflowStep
{
    public class CreateWorkflowStepRequest
    {
        public int RouteId { get; set; }
        public int SequenceNo { get; set; }
        public string? StepName { get; set; }
        
        // Parallel Execution
        public int? ParallelGroupId { get; set; }
        public ExecutionMode ExecutionMode { get; set; } = ExecutionMode.Sequential;
        public CompletionRule CompletionRule { get; set; } = CompletionRule.Any;
        
        // Return Behavior
        public bool AllowReturn { get; set; } = false;
        public WorkflowReturnBehavior ReturnBehavior { get; set; } = WorkflowReturnBehavior.ToPreviousStep;
        public int? ReturnStepId { get; set; }
        
        // Final Step
        public bool IsFinalStep { get; set; } = false;
        
        // Conditional Routing
        public NextStepConditionDto? NextStepCondition { get; set; }
        
        // Assignments
        public List<CreateWorkflowStepAssignmentRequest> Assignments { get; set; } = new();
    }
}
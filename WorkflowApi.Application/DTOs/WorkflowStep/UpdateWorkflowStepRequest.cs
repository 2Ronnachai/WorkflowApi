using WorkflowApi.Application.DTOs.Condition;
using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Application.DTOs.WorkflowStep
{
    public class UpdateWorkflowStepRequest
    {
        public string? StepName { get; set; }
        public int? ParallelGroupId { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public CompletionRule CompletionRule { get; set; }
        public bool AllowReturn { get; set; }
        public WorkflowReturnBehavior ReturnBehavior { get; set; }
        public int? ReturnStepId { get; set; }
        public bool IsFinalStep { get; set; }
        public NextStepConditionDto? NextStepCondition { get; set; }
    }
}
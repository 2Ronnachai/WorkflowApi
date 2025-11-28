using WorkflowApi.Application.DTOs.Condition;
using WorkflowApi.Application.DTOs.WorkflowStepAssignment;

namespace WorkflowApi.Application.DTOs.WorkflowStep
{
    public class WorkflowStepResponse
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int SequenceNo { get; set; }
        public string? StepName { get; set; }
        
        public int? ParallelGroupId { get; set; }
        public string ExecutionMode { get; set; } = string.Empty;  // "Sequential" | "Parallel"
        public string CompletionRule { get; set; } = string.Empty; // "Any" | "All" | "Majority"
        
        public bool AllowReturn { get; set; }
        public string ReturnBehavior { get; set; } = string.Empty;
        public int? ReturnStepId { get; set; }
        public string? ReturnStepName { get; set; } // Populated from ReturnStep
        
        public bool IsFinalStep { get; set; }
    
        public NextStepConditionDto? NextStepCondition { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public List<WorkflowStepAssignmentResponse> Assignments { get; set; } = new();
    }
}
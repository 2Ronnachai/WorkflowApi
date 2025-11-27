namespace WorkflowApi.Application.DTOs.Resolution
{
    public class ResolvedWorkflowStepResponse
    {
        public int StepId { get; set; }
        public int SequenceNo { get; set; }
        public string? StepName { get; set; }
        
        // Parallel Execution
        public int? ParallelGroupId { get; set; }
        public string ExecutionMode { get; set; } = string.Empty;
        public string CompletionRule { get; set; } = string.Empty;
        
        // Return Options
        public bool AllowReturn { get; set; }
        public string ReturnBehavior { get; set; } = string.Empty;
        public int? ReturnStepId { get; set; }
        
        // Final Step
        public bool IsFinalStep { get; set; }
        
        // Resolved Assignees (ผลลัพธ์สำคัญ!)
        public List<ResolvedAssigneeResponse> Assignees { get; set; } = new();
        
        // Next Step Info (if conditional)
        public int? NextStepId { get; set; }
        public string? NextStepConditionResult { get; set; } // "Condition met: amount > 10000"
    }
}
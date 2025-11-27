namespace WorkflowApi.Application.DTOs.Condition
{
    public class EvaluateConditionResponse
    {
        public int? NextStepId { get; set; }
        public bool ConditionMet { get; set; } // "amount > 1000" => true
        public string? MatchedRule { get; set; }
        public string? EvaluationDetails { get; set; }
    }
}
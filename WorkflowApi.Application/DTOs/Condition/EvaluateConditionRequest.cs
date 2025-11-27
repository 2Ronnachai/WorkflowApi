namespace WorkflowApi.Application.DTOs.Condition
{
    public class EvaluateConditionRequest
    {
        public int StepId { get; set; }
        public Dictionary<string, object> DocumentData { get; set; } = [];
    }
}
namespace WorkflowApi.Application.DTOs.WorkflowStep
{
    public class ConditionRuleDto
    {
        public string Field { get; set; } = string.Empty;      // "Amount", "Department"
        public string Operator { get; set; } = string.Empty;   // ">", "==", "<", ">=", "<=", "!="
        public object Value { get; set; } = new();             // 1000, "HR"
        public int NextStepId { get; set; }
    }
}
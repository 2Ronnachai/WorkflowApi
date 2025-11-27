namespace WorkflowApi.Application.DTOs.Condition
{
    public class ConditionRuleDto
    {
        public string Field { get; set; } = string.Empty;      // "Amount", "Department"
        public string Operator { get; set; } = string.Empty;   // ">", "==", "<", ">=", "<=", "!="
        public object Value { get; set; } = new();             // 1000, "HR"
        public string? DataType { get; set; }               // "Numeric", "String", "Date"
        public int NextStepId { get; set; }
    }
}
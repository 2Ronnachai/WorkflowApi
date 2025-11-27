namespace WorkflowApi.Application.DTOs.Condition
{
    public class NextStepConditionDto
    {
        public List<ConditionRuleDto> Conditions { get; set; } = [];
        public int DefaultNextStepId { get; set; }
    }
}
namespace WorkflowApi.Application.DTOs.WorkflowStep
{
    public class NextStepConditionDto
    {
        public List<ConditionRuleDto> Conditions { get; set; } = [];
        public int DefaultNextStepId { get; set; }
    }
}
using WorkflowApi.Application.DTOs.Condition;

namespace WorkflowApi.Application.Interfaces
{
    public interface IConditionEvaluator
    {
        EvaluateConditionResponse Evaluate(
            string conditionJson, 
            Dictionary<string, object> documentData);
    }
}
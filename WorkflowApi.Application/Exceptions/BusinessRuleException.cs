namespace WorkflowApi.Application.Exceptions
{
    public class BusinessRuleException(string message) : WorkflowException(
            message,
            422,
            "BUSINESS_RULE_VIOLATION")
    {
        
    }
}
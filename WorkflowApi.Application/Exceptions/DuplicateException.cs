namespace WorkflowApi.Application.Exceptions
{
    public class DuplicateException(string message) : WorkflowException(
            message,
            409,
            "DUPLICATE_RESOURCE")
    {
        
    }
}
namespace WorkflowApi.Application.Exceptions
{
    public class NotFoundException(string resource, object key) : WorkflowException(
            $"{resource} with Id '{key}' not found",
            404,
            "RESOURCE_NOT_FOUND")
    {
        
    }
}
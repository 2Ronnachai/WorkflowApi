namespace WorkflowApi.Application.Exceptions
{
    public class ValidationException(IDictionary<string, string[]> errors) : WorkflowException(
            "One or more validation errors occurred",
            400,
            "VALIDATION_ERROR")
    {
        public IDictionary<string, string[]> Errors { get; } = errors;
    }
}
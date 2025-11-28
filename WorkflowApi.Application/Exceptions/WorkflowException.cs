namespace WorkflowApi.Application.Exceptions
{
    public abstract class WorkflowException(
        string message,
        int statusCode,
        string errorCode) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
        public string ErrorCode { get; } = errorCode;
    }
}
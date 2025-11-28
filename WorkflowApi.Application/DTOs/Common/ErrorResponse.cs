namespace WorkflowApi.Application.DTOs.Common
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? TraceId { get; set; }
        public IDictionary<string, string[]>? ValidationErrors { get; set; }
        
        // For development only
        public string? StackTrace { get; set; }
    }
}
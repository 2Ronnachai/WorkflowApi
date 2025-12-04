namespace WorkflowApi.Application.DTOs.Common
{
    public abstract class AuditDto : BaseDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }
}
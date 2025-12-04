namespace WorkflowApi.Application.DTOs.ExternalResponse
{
    public class PositionDto
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int PositionLevel { get; set; }
        public int? ReportsToPositionId { get; set; }
        public string? ReportsToPositionTitle { get; set; }
    }
}
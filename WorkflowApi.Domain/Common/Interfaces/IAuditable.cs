namespace WorkflowApi.Domain.Common.Interfaces
{
    public interface IAuditable
    {
        string CreatedBy { get; set; }
        string? UpdatedBy { get; set; }
    }
}
namespace WorkflowApi.Domain.Common.Interfaces
{
    public interface ITraceable
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
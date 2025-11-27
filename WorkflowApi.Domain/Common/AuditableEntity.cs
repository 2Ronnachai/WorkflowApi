using WorkflowApi.Domain.Common.Interfaces;

namespace WorkflowApi.Domain.Common
{
    public abstract class AuditableEntity : Entity, ITraceable, IAuditable
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }

    public abstract class AuditableEntity<TKey> : Entity<TKey>, ITraceable, IAuditable
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }
}
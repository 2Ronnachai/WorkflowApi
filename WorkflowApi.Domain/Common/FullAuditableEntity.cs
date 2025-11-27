using WorkflowApi.Domain.Common.Interfaces;

namespace WorkflowApi.Domain.Common
{
    public abstract class FullAuditableEntity : AuditableEntity, ISoftDeletable, IActivatable
    {
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public abstract class FullAuditableEntity<TKey> : AuditableEntity<TKey>, ISoftDeletable, IActivatable
    {
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
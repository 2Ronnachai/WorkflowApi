using WorkflowApi.Domain.Common.Interfaces;

namespace WorkflowApi.Domain.Common
{
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }
    }

    public abstract class Entity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; } = default!;
    }
}
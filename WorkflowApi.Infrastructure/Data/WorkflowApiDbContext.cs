using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Domain.Common.Interfaces;
using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Infrastructure.Data
{
    public class WorkflowApiDbContext(
        DbContextOptions<WorkflowApiDbContext> options,
        ICurrentUserService? currentUserService = null) : DbContext(options)
    {
        private readonly ICurrentUserService? _currentUserService = currentUserService;

        public DbSet<WorkflowRoute> WorkflowRoutes { get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public DbSet<WorkflowStepAssignment> WorkflowStepAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            throw new NotSupportedException("Use SaveChangesAsync() in Web API.");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInformation()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added 
                        || e.State == EntityState.Modified 
                        || e.State == EntityState.Deleted);

            var currentTime = DateTime.UtcNow;
            var userName = string.IsNullOrWhiteSpace(_currentUserService?.GetUserName()) 
                ? "System" 
                : _currentUserService.GetUserName();

            foreach (var entry in entries)
            {
                // Handle Added
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is ITraceable traceable)
                    {
                        traceable.CreatedAt = currentTime;
                    }

                    if (entry.Entity is IAuditable auditable)
                    {
                        auditable.CreatedBy = userName;
                    }
                }

                // Handle Modified
                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is ITraceable traceable)
                    {
                        traceable.UpdatedAt = currentTime;
                        
                        // Don't modify CreatedAt
                        entry.Property(nameof(ITraceable.CreatedAt)).IsModified = false;
                    }

                    if (entry.Entity is IAuditable auditable)
                    {
                        auditable.UpdatedBy = userName;
                        
                        // Don't modify CreatedBy
                        entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;
                    }
                }

                // Handle Soft Delete
                if (entry.State == EntityState.Deleted)
                {
                    if (entry.Entity is ISoftDeletable softDeletable)
                    {
                        entry.State = EntityState.Modified;
                        softDeletable.IsDeleted = true;
                        softDeletable.DeletedAt = currentTime;
                        softDeletable.DeletedBy = userName;
                    }
                }
            }
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Infrastructure.Data.Configurations
{
    public class WorkflowRouteConfiguration : IEntityTypeConfiguration<WorkflowRoute>
    {
        public void Configure(EntityTypeBuilder<WorkflowRoute> builder)
        {
            builder.ToTable("WorkflowRoutes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RouteName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.DocumentType)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            // Audit fields
            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(x => x.DeletedBy)
                .HasMaxLength(100);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Soft delete & Active
            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasMany(x => x.Steps)
                .WithOne(x => x.Route)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.DocumentType)
                .HasDatabaseName("IX_WorkflowRoutes_DocumentType");

            builder.HasIndex(x => x.IsActive)
                .HasDatabaseName("IX_WorkflowRoutes_IsActive");

            builder.HasIndex(x => x.IsDeleted)
                .HasDatabaseName("IX_WorkflowRoutes_IsDeleted");

            // Unique constraint
            builder.HasIndex(x => new { x.RouteName, x.DocumentType })
                .IsUnique()
                .HasDatabaseName("IX_WorkflowRoutes_RouteName_DocumentType")
                .HasFilter("[IsDeleted] = 0");

            // Global Query Filter
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}

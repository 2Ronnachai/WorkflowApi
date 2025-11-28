using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Infrastructure.Data.Configurations
{
    public class WorkflowDelegationConfiguration : IEntityTypeConfiguration<WorkflowDelegation>
    {
        public void Configure(EntityTypeBuilder<WorkflowDelegation> builder)
        {
            builder.ToTable("WorkflowDelegations");

            builder.HasKey(wd => wd.Id);

            // Properties
            builder.Property(wd => wd.Delegator)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(wd => wd.Delegatee)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(wd => wd.StartDate)
                   .IsRequired();

            builder.Property(wd => wd.EndDate)
                   .IsRequired();

            builder.Property(wd => wd.Scope)
                   .IsRequired();

            builder.Property(wd => wd.DocumentType)
                   .HasMaxLength(500);

            builder.Property(wd => wd.Reason)
                   .HasMaxLength(500);

            // Relationships
            builder.HasOne(wd => wd.Route)
                   .WithMany()
                   .HasForeignKey(wd => wd.RouteId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wd => wd.Step)
                   .WithMany()
                   .HasForeignKey(wd => wd.StepId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(wd => new { wd.Delegator, wd.IsActive, wd.StartDate, wd.EndDate })
                   .HasDatabaseName("IX_WorkflowDelegator_IsActive_DateRange");

            builder.HasIndex(wd => wd.Delegatee)
                   .HasDatabaseName("IX_WorkflowDelegatee");
        }
    }
}
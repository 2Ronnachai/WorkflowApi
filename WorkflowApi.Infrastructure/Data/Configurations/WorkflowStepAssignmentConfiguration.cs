using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Infrastructure.Data.Configurations
{
    public class WorkflowStepAssignmentConfiguration : IEntityTypeConfiguration<WorkflowStepAssignment>
    {
        public void Configure(EntityTypeBuilder<WorkflowStepAssignment> builder)
        {
            builder.ToTable("WorkflowStepAssignments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.StepId)
                .IsRequired();

            // Enums as integers
            builder.Property(x => x.AssignmentType)
                .IsRequired()
                .HasConversion<int>();

            // Position-based fields
            builder.Property(x => x.PositionId);

            builder.Property(x => x.OUResolutionMode)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(OUResolutionMode.FollowOrigin);

            builder.Property(x => x.OrganizationalUnitId);

            // Employee-based field
            builder.Property(x => x.NId)
                .HasMaxLength(50);

            // Relationships
            builder.HasOne(x => x.Step)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.StepId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.StepId)
                .HasDatabaseName("IX_WorkflowStepAssignments_StepId");

            builder.HasIndex(x => x.AssignmentType)
                .HasDatabaseName("IX_WorkflowStepAssignments_AssignmentType");

            builder.HasIndex(x => x.PositionId)
                .HasDatabaseName("IX_WorkflowStepAssignments_PositionId");

            builder.HasIndex(x => x.NId)
                .HasDatabaseName("IX_WorkflowStepAssignments_NId");
        }
    }
}

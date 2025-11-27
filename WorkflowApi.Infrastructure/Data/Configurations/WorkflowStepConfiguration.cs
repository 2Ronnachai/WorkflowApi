using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Infrastructure.Data.Configurations
{
    public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
    {
        public void Configure(EntityTypeBuilder<WorkflowStep> builder)
        {
            builder.ToTable("WorkflowSteps");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RouteId)
                .IsRequired();

            builder.Property(x => x.SequenceNo)
                .IsRequired();

            builder.Property(x => x.StepName)
                .HasMaxLength(500);

            builder.Property(x => x.ParallelGroupId);

            // Enums as integers
            builder.Property(x => x.ExecutionMode)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(ExecutionMode.Sequential);

            builder.Property(x => x.CompletionRule)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(CompletionRule.Any);

            builder.Property(x => x.ReturnBehavior)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(WorkflowReturnBehavior.ToSpecificStep);

            builder.Property(x => x.AllowReturn)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.ReturnStepId);

            builder.Property(x => x.IsFinalStep)
                .IsRequired()
                .HasDefaultValue(false);

            // JSON field
            builder.Property(x => x.NextStepCondition)
                .HasColumnType("nvarchar(max)");

            // Audit fields
            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(x => x.Route)
                .WithMany(x => x.Steps)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ReturnStep)
                .WithMany()
                .HasForeignKey(x => x.ReturnStepId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Assignments)
                .WithOne(x => x.Step)
                .HasForeignKey(x => x.StepId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.RouteId)
                .HasDatabaseName("IX_WorkflowSteps_RouteId");

            builder.HasIndex(x => new { x.RouteId, x.SequenceNo })
                .HasDatabaseName("IX_WorkflowSteps_RouteId_SequenceNo");

            builder.HasIndex(x => x.ParallelGroupId)
                .HasDatabaseName("IX_WorkflowSteps_ParallelGroupId");

            builder.HasIndex(x => x.IsFinalStep)
                .HasDatabaseName("IX_WorkflowSteps_IsFinalStep");
        }
    }
}

using WorkflowApi.Domain.Common;

namespace WorkflowApi.Domain.Entities
{
    public class WorkflowRoute : FullAuditableEntity
    {
        public string RouteName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<WorkflowStep> Steps { get; set; } = [];

        // Business Rules
        public void AddStep(WorkflowStep step)
        {
            if(!IsActive)
            {
                throw new InvalidOperationException("Cannot add steps to an inactive route.");
            }
            Steps.Add(step);
        }

        public void Activate()
        {
            if(IsDeleted)
            {
                throw new InvalidOperationException("Cannot activate a deleted route.");
            }
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public bool ValidateStepSequence()
        {
            var sequence = Steps.Select(s => s.SequenceNo).ToList();
            return  sequence.Count == sequence.Distinct().Count();
        }
    }
}
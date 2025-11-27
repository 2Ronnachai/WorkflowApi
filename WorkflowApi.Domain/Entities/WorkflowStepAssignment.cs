using WorkflowApi.Domain.Common;
using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Domain.Entities
{
    public class WorkflowStepAssignment : Entity
    {
        public int StepId { get; set; }
        public WorkflowStep? Step { get; set; }

        public AssignmentType AssignmentType { get; set; }

        // Position-based assignment
        public int? PositionId { get; set; }

        // Organizational Unit-based assignment
        public OUResolutionMode OUResolutionMode { get; set; } = OUResolutionMode.FollowOrigin;
        public int? OrganizationalUnitId { get; set; }

        // Specific employee assignment
        public string? NId { get; set; }

        public void Validate()
        {
            if (AssignmentType == AssignmentType.Position && PositionId == null)
            {
                throw new InvalidOperationException(
                    "PositionId is required for Position-based assignment");
            }
            
           if(AssignmentType == AssignmentType.SpecificEmployee && string.IsNullOrWhiteSpace(NId))
           {
                throw new InvalidOperationException(
                    "NId is required for Specific Employee assignment");
           }

           if(PositionId.HasValue && !string.IsNullOrWhiteSpace(NId))
           {
                throw new InvalidOperationException(
                    "Cannot set both PositionId and NId. Choose one based on AssignmentType.");
           }

           // Validate OUResolutionMode only for Position-based assignments
            if (AssignmentType == AssignmentType.Position)
            {
                if (OUResolutionMode == OUResolutionMode.Fixed && OrganizationalUnitId == null)
                {
                    throw new InvalidOperationException(
                        "OrganizationalUnitId is required when OUResolutionMode is Fixed");
                }

                if(OUResolutionMode != OUResolutionMode.Fixed && OrganizationalUnitId.HasValue)
                {
                    throw new InvalidOperationException(
                         "OrganizationalUnitId should not be set unless OUResolutionMode is Fixed");
                }
            }
        }
    }
}
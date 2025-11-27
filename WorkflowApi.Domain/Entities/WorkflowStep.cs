using WorkflowApi.Domain.Common;
using WorkflowApi.Domain.Enums;

namespace WorkflowApi.Domain.Entities
{
    public class WorkflowStep : AuditableEntity
    {
        public int RouteId { get; set; }
        public WorkflowRoute? Route { get; set; }

        public int SequenceNo { get; set; }
        public string? StepName { get; set; }

        // Parallel execution properties
        public int? ParallelGroupId { get; set; }

        // if ParallelGroupId is set, defines how tasks are executed
        public ExecutionMode ExecutionMode { get; set; } = ExecutionMode.Sequential; 
        public CompletionRule CompletionRule { get; set; } = CompletionRule.Any;

        // Return behavior properties
        public bool AllowReturn { get; set; } = false;
        public WorkflowReturnBehavior ReturnBehavior { get; set; } = WorkflowReturnBehavior.ToPreviousStep;
        public int? ReturnStepId { get; set; }
        public WorkflowStep? ReturnStep { get; set; }

        public bool IsFinalStep { get; set; } = false;

        // Conditional Routing (JSON expression)
        public string? NextStepCondition { get; set; }

        // {
        //     "conditions": [
        //         { "field": "Amount", "operator": ">", "value": 1000, "nextStepId": 3 },
        //         { "field": "Department", "operator": "==", "value": "HR", "nextStepId": 4 }
        //     ],
        //     "defaultNextStepId": 5
        // }

        public ICollection<WorkflowStepAssignment> Assignments { get; set; } = [];

        // Business Rules
        public void ValidateReturnBehavior()
        {
            if(!AllowReturn && ReturnStepId.HasValue)
            {
                throw new InvalidOperationException("ReturnStepId cannot be set when AllowReturn is false.");
            }

            if (AllowReturn)
            {
                if (ReturnBehavior == WorkflowReturnBehavior.ToSpecificStep 
                    && ReturnStepId == null)
                {
                    throw new InvalidOperationException(
                        "ReturnStepId must be specified when ReturnBehavior is ToSpecificStep.");
                }

                if (ReturnStepId == Id)
                {
                    throw new InvalidOperationException(
                        "ReturnStepId cannot be the same as the current step.");
                }
            }
        }

        public void ValidateParallelGroup()
        {
            if(ParallelGroupId.HasValue && ExecutionMode != ExecutionMode.Parallel)
            {
                throw new InvalidOperationException("ExecutionMode must be Parallel when ParallelGroupId is set.");
            }

            if(ExecutionMode == ExecutionMode.Parallel && !ParallelGroupId.HasValue)
            {
                throw new InvalidOperationException("ParallelGroupId must be set when ExecutionMode is Parallel.");
            }

            if(ExecutionMode == ExecutionMode.Sequential && CompletionRule != CompletionRule.Any)
            {
                throw new InvalidOperationException("CompletionRule must be Any when ExecutionMode is Sequential.");
            }
        }

        public void ValidateFinalStep()
        {
            if (IsFinalStep)
            {
                if (NextStepCondition != null)
                {
                    throw new InvalidOperationException(
                        "Final step cannot have NextStepCondition.");
                }

                // Final step cannot allow return
                if (AllowReturn)
                {
                    throw new InvalidOperationException(
                        "Final step cannot allow return.");
                }
            }
        }

        public void ValidateSequence()
        {
            if (SequenceNo <= 0)
            {
                throw new InvalidOperationException(
                    "SequenceNo must be a positive integer.");
            }
        }
    }
}
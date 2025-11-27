namespace WorkflowApi.Domain.Enums
{
    public enum WorkflowReturnBehavior
    {
        ToPreviousStep = 1,    // Return to the immediate previous step
        ToInitiator = 2,       // Return to the initiator of the workflow
        ToSpecificStep = 3     // Return to a specific step defined in the workflow
    }
}
namespace WorkflowApi.Domain.Enums
{
    public enum DelegationScope
    {
        All = 0,              // All workflows
        SpecificRoute = 1,    // Specific workflow route
        SpecificDocType = 2,  // Specific document type
        SpecificStep = 3      // Specific step
    }
}
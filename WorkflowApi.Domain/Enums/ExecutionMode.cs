namespace WorkflowApi.Domain.Enums
{
    public enum ExecutionMode
    {
        Sequential = 1,     // Tasks are executed one after another
        Parallel = 2        // Tasks are executed simultaneously
    }
}
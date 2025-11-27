namespace WorkflowApi.Domain.Enums
{
    public enum CompletionRule
    {
        Any = 1,       // Completion when any one task is finished
        All = 2,       // Completion when all tasks are finished
        Majority = 3   // Completion when more than half of the tasks are finished Ex. 3 out of 5
    }
}
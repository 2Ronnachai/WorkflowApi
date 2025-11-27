namespace WorkflowApi.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string GetUserName();
        string GetUserId();
        bool IsAuthenticated();
    }
}
namespace WorkflowApi.Infrastructure.Configurations
{
    public class OrganizationApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
        public string? ApiKey { get; set; }
    }
}
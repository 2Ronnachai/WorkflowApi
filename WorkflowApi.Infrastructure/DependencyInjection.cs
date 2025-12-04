using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Infrastructure.Configurations;
using WorkflowApi.Infrastructure.ExternalServices;

namespace WorkflowApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure settings
            services.Configure<OrganizationApiSettings>(
                configuration.GetSection("ExternalApis:OrganizationApi"));

            // Register HttpClients
            services.AddHttpClient<EmployeeApiService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<OrganizationApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
            });

            services.AddHttpClient<PositionApiService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<OrganizationApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
            });

            services.AddHttpClient<OrganizationUnitApiService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<OrganizationApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
            });

            // Register with Request-Scoped Cache
            services.AddScoped<IEmployeeService>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>()
                    .CreateClient(nameof(EmployeeApiService));
                var logger = sp.GetRequiredService<ILogger<EmployeeApiService>>();
                var apiService = new EmployeeApiService( httpClient, logger);
                
                var cacheLogger = sp.GetRequiredService<ILogger<RequestScopedEmployeeCache>>();
                return new RequestScopedEmployeeCache(apiService, cacheLogger);
            });

            services.AddScoped<IPositionService>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>()
                    .CreateClient(nameof(PositionApiService));
                var logger = sp.GetRequiredService<ILogger<PositionApiService>>();
                var apiService = new PositionApiService( httpClient, logger);
                
                var cacheLogger = sp.GetRequiredService<ILogger<RequestScopedPositionCache>>();
                return new RequestScopedPositionCache(apiService, cacheLogger);
            });

            services.AddScoped<IOrganizationUnitService>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>()
                    .CreateClient(nameof(OrganizationUnitApiService));
                var logger = sp.GetRequiredService<ILogger<OrganizationUnitApiService>>();
                var apiService = new OrganizationUnitApiService( httpClient, logger);
                
                var cacheLogger = sp.GetRequiredService<ILogger<RequestScopedOuCache>>();
                return new RequestScopedOuCache(apiService, cacheLogger);
            });

            return services;
        }
    }
}
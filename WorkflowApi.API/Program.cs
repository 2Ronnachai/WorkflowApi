using Microsoft.EntityFrameworkCore;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Application.Services;
using WorkflowApi.Domain.Interfaces;
using WorkflowApi.Infrastructure.Data;
using WorkflowApi.Infrastructure.Middleware;
using WorkflowApi.Infrastructure.Repositories;
using WorkflowApi.Infrastructure.Services;
using WorkflowApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Database context
builder.Services.AddDbContext<WorkflowApiDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("WorkflowApi.Infrastructure")
        )
    );

builder.Services.AddHttpContextAccessor(); // For ICurrentUserService

// AutoMapper Configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// CORS Configuration
var corsSettings = builder.Configuration.GetSection("Cors");
var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? [];
var allowAnyOrigin = corsSettings.GetValue<bool>("AllowAnyOrigin");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowAnyOrigin)
        {
            // Development: Allow any origin
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else if (allowedOrigins.Length > 0)
        {
            // Production: Restrict to specified origins
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            // Fallback: No origins allowed
            throw new InvalidOperationException(
                "CORS configuration is required!"
            );
        }
    });
});

// Dependency Injection for repositories and unit of work
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Specific repositories
builder.Services.AddScoped<IWorkflowRouteRepository, WorkflowRouteRepository>();
builder.Services.AddScoped<IWorkflowStepRepository, WorkflowStepRepository>();
builder.Services.AddScoped<IWorkflowStepAssignmentRepository, WorkflowStepAssignmentRepository>();
builder.Services.AddScoped<IWorkflowDelegationRepository, WorkflowDelegationRepository>();

// Dependency Injection for application services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Specific application services
builder.Services.AddScoped<IWorkflowRouteService, WorkflowRouteService>();
builder.Services.AddScoped<IWorkflowStepService, WorkflowStepService>();
builder.Services.AddScoped<IWorkflowStepAssignmentService, WorkflowStepAssignmentService>();
builder.Services.AddScoped<IWorkflowResolutionService, WorkflowResolutionService>();

builder.Services.AddScoped<IWorkflowRouteEnrichmentService, WorkflowRouteEnrichmentService>();

// Other application services
builder.Services.AddScoped<IDelegationResolver, DelegationResolverService>();
builder.Services.AddScoped<IConditionEvaluator, ConditionEvaluatorService>();
builder.Services.AddScoped<IAssignmentResolver, AssignmentResolverService>();


// Add Infrastructure (includes EmployeeService)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

using Microsoft.EntityFrameworkCore;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Application.Services;
using WorkflowApi.Domain.Interfaces;
using WorkflowApi.Infrastructure.Data;
using WorkflowApi.Infrastructure.Repositories;
using WorkflowApi.Infrastructure.Services;

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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Dependency Injection for repositories and unit of work
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Specific repositories
builder.Services.AddScoped<IWorkflowRouteRepository, WorkflowRouteRepository>();
builder.Services.AddScoped<IWorkflowStepRepository, WorkflowStepRepository>();
builder.Services.AddScoped<IWorkflowStepAssignmentRepository, WorkflowStepAssignmentRepository>();

// Dependency Injection for application services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Specific application services
builder.Services.AddScoped<IWorkflowRouteService, WorkflowRouteService>();
builder.Services.AddScoped<IWorkflowStepService, WorkflowStepService>();
builder.Services.AddScoped<IWorkflowStepAssignmentService, WorkflowStepAssignmentService>();
builder.Services.AddScoped<IWorkflowResolutionService, WorkflowResolutionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

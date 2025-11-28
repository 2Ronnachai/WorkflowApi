using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using WorkflowApi.Application.DTOs.Common;
using WorkflowApi.Application.Exceptions;

namespace WorkflowApi.Infrastructure.Middleware
{
    public class GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;
        private readonly IHostEnvironment _environment = environment;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log exception
            _logger.LogError(
                exception,
                "An error occurred: {Message}. TraceId: {TraceId}",
                exception.Message,
                context.TraceIdentifier);

            // Prepare response
            var response = CreateErrorResponse(context, exception);
            var statusCode = GetStatusCode(exception);

            // Write response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, options));
        }

        private ErrorResponse CreateErrorResponse(HttpContext context, Exception exception)
        {
            var response = new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case WorkflowException workflowEx:
                    response.ErrorCode = workflowEx.ErrorCode;
                    response.Message = workflowEx.Message;
                    
                    if (workflowEx is ValidationException validationEx)
                    {
                        response.ValidationErrors = validationEx.Errors;
                    }
                    break;

                case KeyNotFoundException:
                    response.ErrorCode = "RESOURCE_NOT_FOUND";
                    response.Message = exception.Message;
                    break;

                case InvalidOperationException:
                    response.ErrorCode = "INVALID_OPERATION";
                    response.Message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    response.ErrorCode = "UNAUTHORIZED";
                    response.Message = "You don't have permission to access this resource";
                    break;

                default:
                    response.ErrorCode = "INTERNAL_SERVER_ERROR";
                    response.Message = _environment.IsDevelopment()
                        ? exception.Message
                        : "An error occurred while processing your request";
                    break;
            }

            // Include stack trace in development
            if (_environment.IsDevelopment())
            {
                response.StackTrace = exception.StackTrace;
            }

            return response;
        }

        private static int GetStatusCode(Exception exception) => exception switch
        {
            WorkflowException workflowEx => workflowEx.StatusCode,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
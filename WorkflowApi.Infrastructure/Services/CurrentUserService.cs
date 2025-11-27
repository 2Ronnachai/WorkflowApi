using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.Infrastructure.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string GetUserName()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return "System";

            // Windows Authentication (Negotiate/NTLM)
            var windowsUser = context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(windowsUser))
            {
                // Extract username from DOMAIN\username format
                var parts = windowsUser.Split('\\');
                return parts.Length > 1 ? parts[1] : parts[0];
            }

            // JWT Claims (fallback)
            var jwtUser = context.User?.FindFirst(ClaimTypes.Name)?.Value
                    ?? context.User?.FindFirst("preferred_username")?.Value;
            
            return jwtUser ?? "System";
        }

        public string GetUserId()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return "0";

            // JWT Claims
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? context.User?.FindFirst("sub")?.Value
                    ?? context.User?.FindFirst("sid")?.Value;

            return userId ?? "0";
        }

        public bool IsAuthenticated()
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.User?.Identity?.IsAuthenticated == true;
        }
    }
}
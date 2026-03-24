using RfidAccessSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace RfidAccessSystem.Middleware
{
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER = "X-API-Key";

        public ApiKeyAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            // Skip auth for health check and static files
            if (context.Request.Path.StartsWithSegments("/api/health") || 
                context.Request.Path.StartsWithSegments("/wwwroot"))
            {
                await _next(context);
                return;
            }

            // Only check API endpoints
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            string? apiKey = context.Request.Headers[API_KEY_HEADER];

            if (string.IsNullOrEmpty(apiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = "Missing API Key in X-API-Key header" });
                return;
            }

            var key = await dbContext.ApiKeys
            .FirstOrDefaultAsync(k => k.KeyValue == apiKey && k.IsActive);

            if (key == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key" });
                return;
            }

            // Check expiration
            if (key.ExpiresAt.HasValue && key.ExpiresAt < DateTime.UtcNow)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = "API Key expired" });
                return;
            }

            // Update last used
            key.LastUsedAt = DateTime.UtcNow;
            dbContext.SaveChanges();

            context.Items["ApiKey"] = apiKey;
            await _next(context);
        }
    }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    status = "error",
                    code = "INTERNAL_ERROR",
                    message = ex.Message
                });
            }
        }
    }
}

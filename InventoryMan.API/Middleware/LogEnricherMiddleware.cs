using Serilog.Context;

namespace InventoryMan.API.Middleware
{
    // InventoryMan.API/Middleware/LogEnricherMiddleware.cs
    public class LogEnricherMiddleware
    {
        private readonly RequestDelegate _next;

        public LogEnricherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (LogContext.PushProperty("CorrelationId", GetOrCreateCorrelationId(context)))
            using (LogContext.PushProperty("UserAgent", context.Request.Headers["User-Agent"].ToString()))
            using (LogContext.PushProperty("ClientIP", context.Connection.RemoteIpAddress?.ToString()))
            {
                await _next(context);
            }
        }

        private string GetOrCreateCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
            {
                return correlationId.ToString();
            }

            var newCorrelationId = Guid.NewGuid().ToString();
            context.Request.Headers.Add("X-Correlation-ID", newCorrelationId);
            return newCorrelationId;
        }
    }

}

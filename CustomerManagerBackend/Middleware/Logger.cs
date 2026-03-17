using System;
using System.Diagnostics;

namespace CustomerManager.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var method = context.Request.Method;
        var path = context.Request.Path;
        var timestamp = DateTime.UtcNow;

        await _next(context);

        stopwatch.Stop();

        var responseTime = stopwatch.ElapsedMilliseconds;

        _logger.LogInformation("{Timestamp} | {Method} {Path} | {ResponseTime} ms",
            timestamp, method, path, responseTime);
    }
}

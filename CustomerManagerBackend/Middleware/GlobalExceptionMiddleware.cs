using System;
using System.Net;
using System.Text.Json;

namespace CustomerManager.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }    

    private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.ContentType = "application/json";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred.";

        if (exception is KeyNotFoundException)
        {
            statusCode = (int)HttpStatusCode.NotFound;
            message = exception.Message;
        }
        else if (exception is InvalidOperationException)
        {
            statusCode = (int)HttpStatusCode.Conflict;
            message = exception.Message;
        }

        httpContext.Response.StatusCode = statusCode;

        var response = new
        {
            status = statusCode,
            error = message
        };

        return httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

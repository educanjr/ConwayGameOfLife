
using ConwayGameOfLife.Web.Common;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ConwayGameOfLife.Web.Middleware;

public class ExeptionProcessMiddleware
{
    private readonly RequestDelegate _next;

    public ExeptionProcessMiddleware(RequestDelegate next)
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = ErrorResponseFactory.BuildErrorResponse(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.Status!.Value;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

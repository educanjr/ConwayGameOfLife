using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConwayGameOfLife.Web.Common;

public static class ErrorResponseFactory
{
    public static (HttpStatusCode, ProblemDetails) BuildErrorResponse(Exception exception)
    {
        var statusCode = exception is InvalidOperationException
            ? HttpStatusCode.BadRequest
            : HttpStatusCode.InternalServerError;

        var response = ResponsesGenerationUtil.CreateProblemDetails(
                    "A problem was found",
                    statusCode.ToString(),
                    exception.Message,
                    (int)statusCode);

        return (statusCode, response);
    }
}

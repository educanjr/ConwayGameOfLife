using Microsoft.AspNetCore.Mvc;

namespace ConwayGameOfLife.Web.Common;

internal static class ResponsesGenerationUtil
{
    public static ProblemDetails CreateProblemDetails(
        string title,
        string type,
        string detail,
        int status) => new()
        {
            Title = title,
            Type = type,
            Detail = detail,
            Status = status
        };
}

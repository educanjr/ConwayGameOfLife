using ConwayGameOfLife.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ConwayGameOfLife.Web.Common;

internal static class ResponsesGenerationUtil
{
    public static ProblemDetails CreateProblemDetails(string title, string type, string detail, int status) => new()
    {
        Title = title,
        Type = type,
        Detail = detail,
        Status = status
    };

    public static ProblemDetails CreateProblemDetails(ResultError error, int statusCode) => new()
    {
        Title = $"{error.Target} {error.Message}",
        Type = error.Code.ToString(),
        Detail = error.Details is null ? string.Empty : string.Join(Environment.NewLine, error.Details!.Select(x => x.Message)),
        Status = statusCode,
    };
}

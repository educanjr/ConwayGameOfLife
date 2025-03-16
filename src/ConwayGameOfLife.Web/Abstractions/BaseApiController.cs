using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Web.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace ConwayGameOfLife.Web.Abstractions;

public abstract class BaseApiController<TController> : ControllerBase
    where TController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<TController> _logger;

    protected ISender Sender => _sender;
    protected ILogger<TController> Logger => _logger;

    protected BaseApiController(ISender sender, ILogger<TController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    protected IActionResult HandleFailure(ResultError error) =>
        error.Code switch
        {
            ErrorCode.NotFound => NotFound(
                ResponsesGenerationUtil.CreateProblemDetails(error, StatusCodes.Status400BadRequest)),
            ErrorCode.InternalError => StatusCode(
                StatusCodes.Status500InternalServerError,
                ResponsesGenerationUtil.CreateProblemDetails(error, StatusCodes.Status500InternalServerError)),
            _ => throw new InvalidOperationException(),
        };

    protected IActionResult HandleError(Exception error) =>
        StatusCode(
            StatusCodes.Status500InternalServerError,
            ResponsesGenerationUtil.CreateProblemDetails(
                "Bad Request",
                StatusCodes.Status400BadRequest.ToString(),
                error.Message,
                StatusCodes.Status400BadRequest));

    protected static Task NotImplementedEndpointPlaceholder() =>
        Task.FromException(new NotImplementedException());

    protected string GenerateHateoasUrl(string endpointName, object? routeValues)
    {
        var linkGenerator = HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
        return linkGenerator.GetUriByName(HttpContext, endpointName, routeValues) ?? string.Empty;
    }
}

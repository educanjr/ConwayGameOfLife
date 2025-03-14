using ConwayGameOfLife.Web.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    protected IActionResult HandleFailure(Exception error) =>
        BadRequest(
                ResponsesGenerationUtil.CreateProblemDetails(
                    "Bad Request",
                    Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest.ToString(),
                    error.Message,
                    Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest
            ));

    protected static Task NotImplementedEndpointPlaceholder() =>
        Task.FromException(new NotImplementedException());

    protected string GenerateHateoasUrl(string endpointName, object? routeValues)
    {
        var linkGenerator = HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
        return linkGenerator.GetUriByName(HttpContext, endpointName, routeValues) ?? string.Empty;
    }
}

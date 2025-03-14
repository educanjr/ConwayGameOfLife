using ConwayGameOfLife.Web.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace ConwayGameOfLife.Controllers;

[Asp.Versioning.ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]

public class GameController : BaseApiController<GameController>
{
    public GameController(ISender sender, ILogger<GameController> logger)
        :base(sender, logger)
    {
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBoard()
    {
        try
        {
            await NotImplementedEndpointPlaceholder();
            return Created(GenerateHateoasUrl(nameof(GetBoardNextState), Guid.Empty), Guid.Empty);
        }
        catch (Exception ex)
        {
            return HandleFailure(ex);
        }
    }

    [HttpGet("{id}/next")]
    [EndpointName(nameof(GetBoardNextState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBoardNextState(Guid id)
    {
        try
        {
            await NotImplementedEndpointPlaceholder();
            return Ok();
        }
        catch (Exception ex)
        {
            return HandleFailure(ex);
        }
    }

    [HttpGet("{id}/next/{steps}")]
    [EndpointName(nameof(GetBoardNextStepsState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBoardNextStepsState(Guid id, uint steps)
    {
        try
        {
            await NotImplementedEndpointPlaceholder();
            return Ok();
        }
        catch (Exception ex)
        {
            return HandleFailure(ex);
        }
    }

    [HttpGet("{id}/final")]
    [EndpointName(nameof(GetBoardFinalState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBoardFinalState(Guid id)
    {
        try
        {
            await NotImplementedEndpointPlaceholder();
            return Ok();
        }
        catch (Exception ex)
        {
            return HandleFailure(ex);
        }
    }
}

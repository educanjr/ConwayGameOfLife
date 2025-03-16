using ConwayGameOfLife.Application.CommandAndQueries.Board.GetCurrent;
using ConwayGameOfLife.Application.CommandAndQueries.Board.GetStep;
using ConwayGameOfLife.Application.CommandAndQueries.Board.Register;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Web.Abstractions;
using ConwayGameOfLife.Web.Common;
using ConwayGameOfLife.Web.Contracts;
using ConwayGameOfLife.Web.Extensions;
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest request)
    {
        try
        {
            return await ResultObject
                .Create(new RegisterBoardCommand(request.Name, BoardState.FromJaggedArray(request.State)))
                .Bind(cmd => Sender.Send(cmd))
                .Match(
                    id => Created(GenerateHateoasUrl(nameof(GetCurrentBoardState), id), id),
                    HandleFailure);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpGet("{id}")]
    [EndpointName(nameof(GetCurrentBoardState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentBoardState(Guid id)
    {
        try
        {
            return await ResultObject
                .Create(new GetCurrentBoardQuery(id))
                .Bind(cmd => Sender.Send(cmd))
                .Match(
                    board => Ok(DataConverters.CurrentBoardStateConverter(board)),
                    HandleFailure);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpGet("{id}/steps/{step}")]
    [EndpointName(nameof(GetBoardState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBoardState(Guid id, uint step)
    {
        try
        {
            return await ResultObject
                .Create(new GetBoardStepQuery(id, step))
                .Bind(cmd => Sender.Send(cmd))
                .Match(
                    board => Ok(DataConverters.BoardStateConverter(board, step)),
                    HandleFailure);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpPatch("{id}/next")]
    [EndpointName(nameof(CalculateBoardNextState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CalculateBoardNextState(Guid id)
    {
        try
        {
            await NotImplementedEndpointPlaceholder();
            return Ok();
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpPatch("{id}/next/{steps}")]
    [EndpointName(nameof(CalculateBoardNextStepsState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CalculateBoardNextStepsState(Guid id, uint steps)
    {
        try
        {
            await NotImplementedEndpointPlaceholder();
            return Ok();
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpGet("{id}/final")]
    [EndpointName(nameof(GetBoardFinalState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBoardFinalState(Guid id)
    {
        try
        {
            await NotImplementedEndpointPlaceholder();
            return Ok();
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }
}

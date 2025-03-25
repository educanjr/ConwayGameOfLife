using ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateFinalStep;
using ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateNextNSteps;
using ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateNextStep;
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

/// <summary>
/// API controller for managing Game of Life boards, executions, and simulation states.
/// </summary>
[Asp.Versioning.ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]

public class GameController : BaseApiController<GameController>
{
    public GameController(ISender sender, ILogger<GameController> logger)
        :base(sender, logger)
    {
    }

    // <summary>
    /// Registers a new Game of Life board with an initial state.
    /// </summary>
    /// <param name="request">The board name and initial state.</param>
    /// <returns>201 Created with the board ID, or appropriate error response.</returns>
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

    /// <summary>
    /// Gets the latest known state of a board (either initial or most recent execution).
    /// </summary>
    /// <param name="id">Board ID.</param>
    /// <returns>The current board state.</returns>
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

    /// <summary>
    /// Gets the board state at a specific step in the simulation.
    /// </summary>
    /// <param name="id">Board ID.</param>
    /// <param name="step">Step number to retrieve.</param>
    /// <returns>The board state at the requested step.</returns>
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

    /// <summary>
    /// Calculates the next state in the board's evolution.
    /// </summary>
    /// <param name="id">Board ID.</param>
    /// <returns>The board state after the next step.</returns>
    [HttpPatch("{id}/next")]
    [EndpointName(nameof(CalculateBoardNextState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CalculateBoardNextState(Guid id)
    {
        try
        {
            return await ResultObject
                .Create(new CalculateNextStepCommand(id))
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

    /// <summary>
    /// Calculates the next N steps of the board evolution or until a final state is reached.
    /// </summary>
    /// <param name="id">Board ID.</param>
    /// <param name="steps">Number of steps to calculate.</param>
    /// <returns>The updated board state.</returns>
    [HttpPatch("{id}/next/{steps}")]
    [EndpointName(nameof(CalculateBoardNextStepsState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CalculateBoardNextStepsState(Guid id, int steps)
    {
        if (steps <= 0)
        {
            return BadRequest(new
            {
                errors = new
                {
                    Steps = new[] { "The number of steps must be greater than 0." }
                }
            });
        }

        try
        {
            return await ResultObject
                .Create(new CalculateNextNStepsCommand(id, steps))
                .Bind(cmd => Sender.Send(cmd))
                .Match(
                    board => Ok(DataConverters.CalculatedBoardStateConverter(board)),
                    HandleFailure);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    /// <summary>
    /// Calculates all remaining steps until the board reaches a final state or hits the step limit.
    /// </summary>
    /// <param name="id">Board ID.</param>
    /// <returns>The final board state.</returns>
    [HttpGet("{id}/final")]
    [EndpointName(nameof(GetBoardFinalState))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBoardFinalState(Guid id)
    {
        try
        {
            return await ResultObject
                .Create(new CalculateFinalStepCommand(id))
                .Bind(cmd => Sender.Send(cmd))
                .Match(
                    board => Ok(DataConverters.CalculatedBoardStateConverter(board)),
                    HandleFailure);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }
}

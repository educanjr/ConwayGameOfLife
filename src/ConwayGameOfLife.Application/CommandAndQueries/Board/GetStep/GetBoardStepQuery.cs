using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Dtos;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.GetStep;

/// <summary>
/// Query that retrieves the state of a board at a specific step.
/// </summary>
/// <param name="Id">The ID of the board to inspect.</param>
/// <param name="Step">The step number to retrieve.</param>
public sealed record GetBoardStepQuery(Guid Id, uint Step) : IQuery<BoardStateDto>;

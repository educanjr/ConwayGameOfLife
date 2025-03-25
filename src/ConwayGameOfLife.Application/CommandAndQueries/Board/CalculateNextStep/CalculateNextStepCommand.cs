using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Dtos;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateNextStep;

/// <summary>
/// Command that computes the next execution step for the specified board.
/// </summary>
/// <param name="Id">The ID of the board to process.</param>
public sealed record CalculateNextStepCommand(Guid Id) : ICommand<BoardStateDto>;

using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Dtos;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateFinalStep;

/// <summary>
/// Command that calculates all remaining steps of the Game of Life board until it reaches a final state
/// or the execution limit is hit.
/// </summary>
/// <param name="Id">The ID of the board to process.</param>
public sealed record CalculateFinalStepCommand(Guid Id) : ICommand<CalculateExecutionsDto>;

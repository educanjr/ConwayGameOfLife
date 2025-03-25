using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Dtos;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateNextNSteps;

/// <summary>
/// Command that calculates a specific number of steps for the board, stopping early if a final state is reached.
/// </summary>
/// <param name="Id">The ID of the board to process.</param>
/// <param name="Steps">The number of steps to calculate.</param>
public sealed record CalculateNextNStepsCommand(Guid Id, int Steps) : ICommand<CalculateExecutionsDto>;

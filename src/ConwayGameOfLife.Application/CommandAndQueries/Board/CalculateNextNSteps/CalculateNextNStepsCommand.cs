using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Dtos;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateNextNSteps;

public sealed record CalculateNextNStepsCommand(Guid Id, int Steps) : ICommand<CalculateExecutionsDto>;

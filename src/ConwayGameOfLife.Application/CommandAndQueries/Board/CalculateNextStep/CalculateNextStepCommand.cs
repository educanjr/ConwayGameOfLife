using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Dtos;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateNextStep;

public sealed record CalculateNextStepCommand(Guid Id) : ICommand<BoardStateDto>;

using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Dtos;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateFinalStep;

public sealed record CalculateFinalStepCommand(Guid Id) : ICommand<BoardStateDto>;

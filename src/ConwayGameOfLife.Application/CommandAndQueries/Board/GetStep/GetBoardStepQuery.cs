using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Dtos;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.GetStep;

public sealed record GetBoardStepQuery(Guid Id, uint Step) : IQuery<BoardStateDto>;

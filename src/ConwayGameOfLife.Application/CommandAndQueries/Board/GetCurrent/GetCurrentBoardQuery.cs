using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Dtos;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.GetCurrent;

public sealed record GetCurrentBoardQuery(Guid Id) : IQuery<CurrentBoardStateDto>;

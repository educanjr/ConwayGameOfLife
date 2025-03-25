using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Dtos;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.GetCurrent;

/// <summary>
/// Query that retrieves the most recent execution (or initial state) of a board.
/// </summary>
/// <param name="Id">The ID of the board to retrieve.</param>
public sealed record GetCurrentBoardQuery(Guid Id) : IQuery<BoardStateDto>;

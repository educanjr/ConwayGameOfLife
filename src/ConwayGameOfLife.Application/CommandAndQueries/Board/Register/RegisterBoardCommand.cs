using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Entities;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.Register;

/// <summary>
/// Command that registers a new Game of Life board with the given name and initial state.
/// </summary>
/// <param name="Name">The display name of the board.</param>
/// <param name="State">The initial cell state of the board.</param>
public sealed record RegisterBoardCommand(string Name, BoardState State) : ICommand<Guid>;

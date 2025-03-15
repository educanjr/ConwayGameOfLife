using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Entities;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.Register;

public sealed record RegisterBoardCommand(string Name, BoardState State) : ICommand<Guid>;

using ConwayGameOfLife.Application.Entities;

namespace ConwayGameOfLife.Application.Repositories;

public interface IBoardRepository
{
    ValueTask<Board> RegisterBoard(string boardName, BoardState initialState);
}

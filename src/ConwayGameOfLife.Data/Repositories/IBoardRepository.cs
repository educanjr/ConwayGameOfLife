using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Application.Repositories;

namespace ConwayGameOfLife.Data.Repositories;

public class BoardRepository : IBoardRepository
{
    public async ValueTask<Board> RegisterBoard(string boardName, BoardState initialState)
    {
        throw new NotImplementedException();
    }
}

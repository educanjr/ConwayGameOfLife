using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Application.Repositories;
using ConwayGameOfLife.Data.Abstractions;

namespace ConwayGameOfLife.Data.Repositories;

public class BoardRepository : BaseRepository, IBoardRepository
{
    public BoardRepository(IConwayDbContext context) : base(context)
    {
    }

    public async ValueTask<Board> RegisterBoard(string boardName, BoardState initialState)
    {
        var board = new Board()
        {
            Name = boardName,
            InitialState = initialState
        };

        ConwayDbContext.Boards.Add(board);

        await ConwayDbContext.SaveChangesAsync();

        return board;
    }
}

using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Application.Repositories;
using ConwayGameOfLife.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

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

    public async ValueTask<Board?> GetBoardIncludingOnlyCurrentExecution(Guid id)
    {
        //EF deal with NULL references in this case
#pragma warning disable CS8604 // Possible null reference argument.
        var board = await ConwayDbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new Board
            {
                Name = x.Name,
                Id = x.Id,
                InitialState = x.InitialState,
                Executions = x.Executions.OrderByDescending(x => x.Step).Take(1).ToList()
            })
            .FirstOrDefaultAsync();
#pragma warning restore CS8604 // Possible null reference argument.

        return board ?? default!;
    }

    public async ValueTask<Board?> GetBoardIncludingExecution(Guid id, uint executionStep)
    {
        //EF deal with NULL references in this case
#pragma warning disable CS8604 // Possible null reference argument.
        var board = await ConwayDbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new Board
            {
                Name = x.Name,
                Id = x.Id,
                InitialState = x.InitialState,
                Executions = x.Executions.Where(x => x.Step == executionStep).Take(1).ToList()
            })
            .FirstOrDefaultAsync();
#pragma warning restore CS8604 // Possible null reference argument.

        return board ?? default!;
    }
}

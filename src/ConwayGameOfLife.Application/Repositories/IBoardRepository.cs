using ConwayGameOfLife.Application.Entities;

namespace ConwayGameOfLife.Application.Repositories;

public interface IBoardRepository
{
    ValueTask<Board> RegisterBoard(string boardName, BoardState initialState);

    ValueTask<Board?> GetBoardIncludingOnlyCurrentExecution(Guid id);

    ValueTask<Board?> GetBoardIncludingExecution(Guid id, uint executionStep);

    ValueTask<Board?> GetBoardIncludingExecutions(Guid id);

    ValueTask<BoardExecution?> AddExecution(BoardExecution execution);
}

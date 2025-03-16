using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.Dtos;
using ConwayGameOfLife.Application.Exceptions;
using ConwayGameOfLife.Application.Repositories;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.GetCurrent;

internal sealed class GetCurrentBoardQueryHandler : IQueryHandler<GetCurrentBoardQuery, BoardStateDto>
{
    private readonly IBoardRepository _boardRepository;

    public GetCurrentBoardQueryHandler(IBoardRepository boardRepository) =>
        _boardRepository = boardRepository;

    public async Task<ResultObject<BoardStateDto>> Handle(GetCurrentBoardQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var board = await _boardRepository.GetBoardIncludingOnlyCurrentExecution(request.Id) ??
                throw new DataNotFoundException(nameof(Board), request.Id.ToString());

            var currentExecution = board.GetLatestExecution();

            return new BoardStateDto(
                Id: board.Id,
                Name: board.Name,
                InitialState: board.InitialState,
                CurrentStep: currentExecution?.Step ?? 0,
                IsCompleted: currentExecution?.IsFinal ?? false,
                State: currentExecution?.State ?? board.InitialState);
        }
        catch (Exception ex)
        {
            return ResultObject.NotFound<BoardStateDto>(ex.Message);
        }
    }
}

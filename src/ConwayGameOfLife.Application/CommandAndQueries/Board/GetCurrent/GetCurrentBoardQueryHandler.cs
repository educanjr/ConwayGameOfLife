using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.Dtos;
using ConwayGameOfLife.Application.Exceptions;
using ConwayGameOfLife.Application.Repositories;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.GetCurrent;

internal sealed class GetCurrentBoardQueryHandler : IQueryHandler<GetCurrentBoardQuery, CurrentBoardStateDto>
{
    private readonly IBoardRepository _boardRepository;

    public GetCurrentBoardQueryHandler(IBoardRepository boardRepository) =>
        _boardRepository = boardRepository;

    public async Task<ResultObject<CurrentBoardStateDto>> Handle(GetCurrentBoardQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var board = await _boardRepository.GetBoardIncludingOnlyCurrentExecution(request.Id) ??
                throw new DataNotFoundException(nameof(Board), request.Id.ToString());

            return new CurrentBoardStateDto(
                Id: board.Id,
                Name: board.Name,
                InitialState: board.InitialState,
                CurrentStep: board.GetLatestExecution()?.Step ?? 0,
                IsCompleted: board.GetLatestExecution()?.IsFinal ?? false,
                CurrentState: board.GetLatestExecution()?.State ?? board.InitialState);
        }
        catch (Exception ex)
        {
            return ResultObject.NotFound<CurrentBoardStateDto>(ex.Message);
        }
    }
}

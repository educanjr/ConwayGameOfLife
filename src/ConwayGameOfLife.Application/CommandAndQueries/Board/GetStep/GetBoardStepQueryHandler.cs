using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.Dtos;
using ConwayGameOfLife.Application.Exceptions;
using ConwayGameOfLife.Application.Repositories;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.GetStep;

internal class GetBoardStepQueryHandler : IQueryHandler<GetBoardStepQuery, BoardStateDto>
{
    private readonly IBoardRepository _boardRepository;

    public GetBoardStepQueryHandler(IBoardRepository boardRepository) =>
        _boardRepository = boardRepository;

    public async Task<ResultObject<BoardStateDto>> Handle(GetBoardStepQuery request, CancellationToken cancellationToken)
    {
        try
        {

            var board = await _boardRepository.GetBoardIncludingExecution(request.Id, request.Step) ??
                throw new DataNotFoundException(nameof(Board), request.Id.ToString());

            var execution = board.GetExecution(request.Step) ??
                throw new DataNotFoundException(nameof(Board), request.Step.ToString());

            var latestStep = execution.Step;
            var isCompleted = execution.IsFinal;

            //If Game is not ended in the requested step, maybe that step is not the last one
            if (!isCompleted)
            {
                var currentBoard = await _boardRepository.GetBoardIncludingOnlyCurrentExecution(request.Id) ??
                    throw new DataNotFoundException(nameof(Board), request.Id.ToString());
                var latestExecution = currentBoard.GetLatestExecution() ??
                    throw new DataNotFoundException(nameof(Board), request.Id.ToString());

                latestStep = latestExecution.Step;
                isCompleted = latestExecution.IsFinal;
            }

            return new BoardStateDto(
                Id: board.Id,
                Name: board.Name,
                InitialState: board.InitialState,
                CurrentStep: latestStep,
                IsCompleted: isCompleted,
                State: execution.State);
        }
        catch (Exception ex)
        {
            return ResultObject.NotFound<BoardStateDto>(ex.Message);
        }
    }
}

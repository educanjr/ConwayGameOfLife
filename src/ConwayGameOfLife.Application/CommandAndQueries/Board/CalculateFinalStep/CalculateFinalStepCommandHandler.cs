using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.Dtos;
using ConwayGameOfLife.Application.Exceptions;
using ConwayGameOfLife.Application.Repositories;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateFinalStep;

internal sealed class CalculateFinalStepCommandHandler : ICommandHandler<CalculateFinalStepCommand, BoardStateDto>
{
    private readonly IBoardRepository _boardRepository;

    public CalculateFinalStepCommandHandler(IBoardRepository boardRepository) =>
        _boardRepository = boardRepository;

    public async Task<ResultObject<BoardStateDto>> Handle(CalculateFinalStepCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var board = await _boardRepository.GetBoardIncludingExecutions(request.Id) ??
                throw new DataNotFoundException(nameof(Board), request.Id.ToString());
            
            var finalExecution = board.ResolveFinalExecution(3);

            var unsavedExecutions = board.Executions!.Where(x => x.Id == Guid.Empty).ToList();
            await _boardRepository.AddExecutionsRange(unsavedExecutions);

            return new BoardStateDto(
               Id: board.Id,
               Name: board.Name,
               InitialState: board.InitialState,
               CurrentStep: finalExecution.Step,
               IsCompleted: finalExecution.IsFinal,
               State: finalExecution.State);
        }
        catch (Exception ex)
        {
            return ex switch {
                ExecutionLimitReachedException => ResultObject.ApplicationRuleViolation<BoardStateDto>(ex.Message),
                _ => ResultObject.NotFound<BoardStateDto>(ex.Message),
            };
        }
    }
}

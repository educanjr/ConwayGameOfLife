using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.CommandAndQueries.Board.Register;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.Dtos;
using ConwayGameOfLife.Application.Exceptions;
using ConwayGameOfLife.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateNextStep;

internal sealed class CalculateNextStepCommandHandler : ICommandHandler<CalculateNextStepCommand, BoardStateDto>
{
    private readonly IBoardRepository _boardRepository;

    public CalculateNextStepCommandHandler(IBoardRepository boardRepository) =>
        _boardRepository = boardRepository;

    public async Task<ResultObject<BoardStateDto>> Handle(CalculateNextStepCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var board = await _boardRepository.GetBoardIncludingExecutions(request.Id) ??
                throw new DataNotFoundException(nameof(Board), request.Id.ToString());

            var currentExecution = board.GetLatestExecution();

            if (currentExecution is not null && currentExecution.IsFinal)
            {
                return new BoardStateDto(
                   Id: board.Id,
                   Name: board.Name,
                   InitialState: board.InitialState,
                   CurrentStep: currentExecution.Step,
                   IsCompleted: currentExecution.IsFinal,
                   State: currentExecution.State);
            }

            var nextExecution = board.ResolveNextExecution();
            await _boardRepository.AddExecution(nextExecution);

            return new BoardStateDto(
               Id: board.Id,
               Name: board.Name,
               InitialState: board.InitialState,
               CurrentStep: nextExecution.Step,
               IsCompleted: nextExecution.IsFinal,
               State: nextExecution.State);
        }
        catch (Exception ex)
        {
            return ResultObject.NotFound<BoardStateDto>(ex.Message);
        }
    }
}

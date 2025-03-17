﻿using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.Dtos;
using ConwayGameOfLife.Application.Exceptions;
using ConwayGameOfLife.Application.Repositories;

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

            var nextExecution = board.ResolveNextExecution(2);
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
            return ex switch
            {
                ExecutionLimitReachedException => ResultObject.ApplicationRuleViolation<BoardStateDto>(ex.Message),
                _ => ResultObject.NotFound<BoardStateDto>(ex.Message),
            };
        }
    }
}

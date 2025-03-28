﻿using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.ConfigOptions;
using ConwayGameOfLife.Application.Dtos;
using ConwayGameOfLife.Application.Exceptions;
using ConwayGameOfLife.Application.Repositories;
using Microsoft.Extensions.Options;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateFinalStep;

internal sealed class CalculateFinalStepCommandHandler : ICommandHandler<CalculateFinalStepCommand, CalculateExecutionsDto>
{
    private readonly IBoardRepository _boardRepository;
    private readonly GameRullerConfig _gameRullerConfig;

    public CalculateFinalStepCommandHandler(IBoardRepository boardRepository, IOptions<GameRullerConfig> gameRullerConfig)
    {
        _boardRepository = boardRepository;
        _gameRullerConfig = gameRullerConfig.Value;
    }

    public async Task<ResultObject<CalculateExecutionsDto>> Handle(CalculateFinalStepCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var board = await _boardRepository.GetBoardIncludingExecutions(request.Id) ??
                throw new DataNotFoundException(nameof(Board), request.Id.ToString());
            
            var finalExecution = board.ResolveFinalExecution(_gameRullerConfig.MaxExecutionsAllowed);

            var unsavedExecutions = board.Executions!.Where(x => x.Id == Guid.Empty).ToList();
            await _boardRepository.AddExecutionsRange(unsavedExecutions);

            return new CalculateExecutionsDto(
               Id: board.Id,
               Name: board.Name,
               InitialState: board.InitialState,
               CurrentStep: finalExecution.Step,
               IsCompleted: finalExecution.IsFinal,
               State: finalExecution.State,
               CalculatedSteps: unsavedExecutions.Count);
        }
        catch (Exception ex)
        {
            return ex switch {
                ExecutionLimitReachedException => ResultObject.ApplicationRuleViolation<CalculateExecutionsDto>(ex.Message),
                _ => ResultObject.NotFound<CalculateExecutionsDto>(ex.Message),
            };
        }
    }
}

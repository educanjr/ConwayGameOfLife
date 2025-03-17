using ConwayGameOfLife.Application.Exceptions;

namespace ConwayGameOfLife.Application.Entities;

public class Board
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public BoardState InitialState { get; set; } = new BoardState();

    public IList<BoardExecution>? Executions { get; set; }

    public BoardExecution? GetLatestExecution()
    {
        if (Executions is null || Executions.Count == 0)
        {
            return null;
        }

        return Executions.OrderByDescending(e => e.Step).FirstOrDefault();
    }

    public BoardExecution? GetExecution(uint executionStep)
    {
        if (Executions is null || Executions.Count == 0)
        {
            return null;
        }

        return Executions.FirstOrDefault(e => e.Step == executionStep);
    }

    public BoardExecution ResolveNextExecution(int maxExecutionsAllowed)
    {
        var latestExecution = GetLatestExecution();
        var currentState = latestExecution?.State ?? InitialState;
        var currentStep = latestExecution?.Step ?? 0;
        var isCompleted = latestExecution?.IsFinal ?? false;

        if (isCompleted || currentStep >= maxExecutionsAllowed)
        {
            throw new ExecutionLimitReachedException();
        }

        var nextState = currentState.ComputeNextState();
        var nextStep = currentStep + 1;
        var isLastState = nextStep == maxExecutionsAllowed || IsLastState(nextState);

        var nextExecution = new BoardExecution
        {
            BoardId = Id,
            Step = nextStep,
            State = nextState,
            IsFinal = isLastState
        };

        AddExecutionToList(nextExecution);

        return nextExecution;
    }

    public BoardExecution ResolveFinalExecution(int maxExecutionsAllowed)
    {
        var execution = GetLatestExecution() ?? ResolveNextExecution(maxExecutionsAllowed);
        var indx = execution.Step;

        while (indx < maxExecutionsAllowed && !execution.IsFinal)
        {
            indx++;
            execution = ResolveNextExecution(maxExecutionsAllowed);
        }

        return execution;
    }

    public BoardExecution ResolveNextExecution(int executionsToResolve, int maxExecutionsAllowed)
    {
        var latestExecution = GetLatestExecution();
        var execution = latestExecution ?? ResolveNextExecution(maxExecutionsAllowed);
        var indx = execution.Step;
        var executionsCounter = latestExecution is null ? 1 : 0;

        while (indx < maxExecutionsAllowed && executionsCounter < executionsToResolve && !execution.IsFinal)
        {
            executionsCounter++;
            indx++;
            execution = ResolveNextExecution(maxExecutionsAllowed);
        }

        return execution;
    }

    private bool IsLastState(BoardState state)
    {
        var initialStateHash = InitialState.GetStateHash();
        var stateHash = state.GetStateHash();

        if (initialStateHash == stateHash)
        {
            return true;
        }

        if (Executions is null || Executions.Count == 0)
        {
            return false;
        }

        return Executions.Any(x => x.State.GetStateHash() == stateHash);
    }

    private void AddExecutionToList(BoardExecution execution)
    {
        if (Executions is null)
        {
            Executions = new List<BoardExecution>()
            {
                execution
            };

            return;
        }

        Executions.Add(execution);
    }
}

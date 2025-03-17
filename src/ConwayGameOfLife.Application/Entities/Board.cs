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

    public BoardExecution ResolveNextExecution()
    {
        var latestExecution = GetLatestExecution();
        var currentState = latestExecution?.State ?? InitialState;
        var currentStep = latestExecution?.Step ?? 0;

        var nextState = currentState.ComputeNextState();
        var isLastState = IsLastState(nextState);

        var nextExecution = new BoardExecution
        {
            BoardId = Id,
            Step = currentStep + 1,
            State = nextState,
            IsFinal = isLastState
        };

        AddExecutionToList(nextExecution);

        return nextExecution;
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

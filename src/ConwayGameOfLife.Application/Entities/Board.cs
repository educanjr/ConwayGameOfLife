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
}

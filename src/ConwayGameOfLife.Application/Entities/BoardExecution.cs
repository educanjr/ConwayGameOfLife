namespace ConwayGameOfLife.Application.Entities;

/// <summary>
/// Represents a single step in a board’s evolution history.
/// </summary>
public class BoardExecution
{
    public Guid Id { get; set; }

    /// <summary>
    /// The numerical step this execution represents in the board's timeline.
    /// </summary>
    public int Step { get; set; }

    /// <summary>
    /// Indicates whether this is the final execution step.
    /// </summary>
    public bool IsFinal { get; set; }

    public BoardState State { get; set; } = new BoardState();

    public Guid BoardId { get; set; }
}

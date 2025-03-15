namespace ConwayGameOfLife.Application.Entities;

public class BoardExecution
{
    public Guid Id { get; set; }
    public int Step { get; set; }
    public bool IsFinal { get; set; }
    public BoardState State { get; set; } = new BoardState();

    public Guid BoardId { get; set; }
}

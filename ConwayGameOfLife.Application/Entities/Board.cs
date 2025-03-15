namespace ConwayGameOfLife.Application.Entities;

public class Board
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public BoardState InitialState { get; set; } = new BoardState();
}

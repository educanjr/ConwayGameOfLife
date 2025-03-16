namespace ConwayGameOfLife.Web.Contracts;

public sealed record CreateBoardRequest(string Name, bool[][] State);

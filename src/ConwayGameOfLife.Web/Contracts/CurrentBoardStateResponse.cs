namespace ConwayGameOfLife.Web.Contracts;

public record CurrentBoardStateResponse(
    Guid Id,
    string Name,
    bool[][] InitialState,
    int CurrentStep,
    bool IsCompleted,
    bool[][] State);

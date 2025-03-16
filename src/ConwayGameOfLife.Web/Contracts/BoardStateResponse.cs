namespace ConwayGameOfLife.Web.Contracts;

public record BoardStateResponse(
    Guid Id, 
    string Name, 
    bool[][] InitialState, 
    int CurrentStep, 
    bool IsCompleted, 
    bool[][] State, 
    uint Step)
    : CurrentBoardStateResponse(Id, Name, InitialState, CurrentStep, IsCompleted, State);

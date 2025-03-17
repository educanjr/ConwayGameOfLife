namespace ConwayGameOfLife.Web.Contracts;

public record CalculatedBoardStateResponse(
    Guid Id, 
    string Name, 
    bool[][] InitialState, 
    int CurrentStep, 
    bool IsCompleted, 
    bool[][] State, 
    int CalculatedSteps)
    : CurrentBoardStateResponse(Id, Name, InitialState, CurrentStep, IsCompleted, State);

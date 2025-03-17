using ConwayGameOfLife.Application.Entities;

namespace ConwayGameOfLife.Application.Dtos;

public record CalculateExecutionsDto(
    Guid Id, 
    string Name, 
    BoardState InitialState, 
    int CurrentStep, 
    bool IsCompleted, 
    BoardState State,
    int CalculatedSteps) 
    : BoardStateDto(Id, Name, InitialState, CurrentStep, IsCompleted, State);

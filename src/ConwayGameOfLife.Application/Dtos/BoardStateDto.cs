using ConwayGameOfLife.Application.Entities;

namespace ConwayGameOfLife.Application.Dtos;

public record BoardStateDto(
    Guid Id,
    string Name,
    BoardState InitialState,
    int CurrentStep,
    bool IsCompleted,
    BoardState State);

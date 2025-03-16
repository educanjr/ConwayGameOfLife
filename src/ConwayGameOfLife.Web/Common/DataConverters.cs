using ConwayGameOfLife.Application.Dtos;
using ConwayGameOfLife.Web.Contracts;

namespace ConwayGameOfLife.Web.Common;

internal static class DataConverters
{
    public static CurrentBoardStateResponse CurrentBoardStateConverter(BoardStateDto applicationData) =>
        new(
            Id: applicationData.Id,
            Name: applicationData.Name,
            InitialState: applicationData.InitialState.ToJaggedArrayState(),
            CurrentStep: applicationData.CurrentStep,
            IsCompleted: applicationData.IsCompleted,
            State: applicationData.State.ToJaggedArrayState());

    public static BoardStateResponse BoardStateConverter(BoardStateDto applicationData, uint requestedStep) =>
        new(
            Id: applicationData.Id,
            Name: applicationData.Name,
            InitialState: applicationData.InitialState.ToJaggedArrayState(),
            CurrentStep: applicationData.CurrentStep,
            IsCompleted: applicationData.IsCompleted,
            State: applicationData.State.ToJaggedArrayState(),
            Step: requestedStep);
}

using ConwayGameOfLife.Application.Dtos;
using ConwayGameOfLife.Web.Contracts;

namespace ConwayGameOfLife.Web.Common;

internal static class DataConverters
{
    public static CurrentBoardStateResponse CurrentBoardStateConverter(CurrentBoardStateDto applicationData) =>
        new(
            Id: applicationData.Id,
            Name: applicationData.Name,
            InitialState: applicationData.InitialState.ToJaggedArrayState(),
            CurrentStep: applicationData.CurrentStep,
            IsCompleted: applicationData.IsCompleted,
            CurrentState: applicationData.CurrentState.ToJaggedArrayState());
}

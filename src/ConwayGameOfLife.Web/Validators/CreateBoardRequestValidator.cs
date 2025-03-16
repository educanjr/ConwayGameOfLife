using ConwayGameOfLife.Web.Contracts;
using FluentValidation;

namespace ConwayGameOfLife.Web.Validators;

public class CreateBoardRequestValidator : AbstractValidator<CreateBoardRequest>
{
    public CreateBoardRequestValidator()
    {
        //Validate Name: Must not be empty
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.");

        //Validate State: Must not be null or empty
        RuleFor(x => x.State)
            .NotNull().WithMessage("State cannot be null.")
            .Must(state => state.Length > 0).WithMessage("State cannot be empty.")
            .Must(AllRowsHaveSameLength).WithMessage("All rows in state must have the same length.");
    }

    //Helper method to check if all rows have the same length
    private bool AllRowsHaveSameLength(bool[][] state)
    {
        if (state == null || state.Length == 0) return false;

        int expectedCols = state[0].Length;
        if (expectedCols == 0) return false;

        return state.All(row => row.Length == expectedCols);
    }
}

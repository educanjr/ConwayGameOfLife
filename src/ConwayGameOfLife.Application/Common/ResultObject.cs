namespace ConwayGameOfLife.Application.Common;

public class ResultObject
{
    protected ResultObject()
    {
        IsSuccess = false;
    }

    private ResultObject(bool succeeded)
        : this()
    {
        IsSuccess = succeeded;
    }

    protected ResultObject(ResultError errorResult)
        : this()
    {
        IsSuccess = false;
        ErrorResult = errorResult;
    }

    public bool IsSuccess { get; protected init; }

    public ResultError? ErrorResult { get; init; }


    public static implicit operator bool(ResultObject value)
    {
        return value.IsSuccess;
    }

    public static implicit operator ResultObject(bool value)
    {
        return new ResultObject(value);
    }

    public static ResultObject Success() => new(true);

    public static ResultObject Error(params string[] errorMessages) => new(
        new ResultError
        {
            Code = ErrorCode.InternalError,
            Message = string.Join(Environment.NewLine, errorMessages)
        });

    public static ResultObject NotFound(params string[] errorMessages) => new(
        new ResultError
        {
            Code = ErrorCode.NotFound,
            Message = string.Join(Environment.NewLine, errorMessages)
        });
}

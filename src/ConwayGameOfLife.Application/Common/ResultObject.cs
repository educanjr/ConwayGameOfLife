namespace ConwayGameOfLife.Application.Common;

public class ResultObject
{
    protected ResultObject()
    {
        IsSuccess = false;
    }

    protected ResultObject(bool succeeded)
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

    public static ResultObject<TResult> Success<TResult>(TResult value) => new(value);

    public static ResultObject Failure(ResultError error) => new(error);

    public static ResultObject<TResult> Failure<TResult>(ResultError error) => new(error);

    public static ResultObject Error(params string[] errorMessages) => new(
        new ResultError
        {
            Code = ErrorCode.InternalError,
            Message = string.Join(Environment.NewLine, errorMessages)
        });

    public static ResultObject<TResult> Error<TResult>(params string[] errorMessages) => new(
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

    public static ResultObject<TResult> NotFound<TResult>(params string[] errorMessages) => new(
       new ResultError
       {
           Code = ErrorCode.NotFound,
           Message = string.Join(Environment.NewLine, errorMessages)
       });

    public static ResultObject<TResult> Create<TResult>(TResult? value) =>
        value is not null ? Success(value) : Error<TResult>("Cannot create a result object with NULL value.");
}

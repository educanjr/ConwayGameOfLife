namespace ConwayGameOfLife.Application.Common;

public class ResultObject<TResult> : ResultObject
{
    public ResultObject()
    {

    }
    private ResultObject(TResult result)
    {
        Result = result;
        IsSuccess = true;
    }

    private ResultObject(ResultError errorResult)
        : base(errorResult)
    {
    }

    public TResult? Result { get; protected set; }

    public static new ResultObject<TResult> Error(params string[] errorMessages) => new ResultObject<TResult>(new ResultError
    {
        Code = ErrorCode.InternalError,
        Message = string.Join(Environment.NewLine, errorMessages)
    });

    public static new ResultObject<TResult> NotFound(params string[] errorMessages) => new ResultObject<TResult>(new ResultError
    {
        Code = ErrorCode.NotFound,
        Message = string.Join(Environment.NewLine, errorMessages)
    });

    public static ResultObject<TResult> Success(TResult result) => new(result);


    public static implicit operator TResult?(ResultObject<TResult> result)
    {
        return result == null ? default : result.Result;
    }

    public static implicit operator ResultObject<TResult>(TResult result)
    {
        return new ResultObject<TResult>(result);
    }
}

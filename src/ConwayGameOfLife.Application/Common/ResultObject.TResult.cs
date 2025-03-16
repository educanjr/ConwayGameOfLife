namespace ConwayGameOfLife.Application.Common;

public class ResultObject<TResult> : ResultObject
{
    private readonly TResult? _value;

    protected internal ResultObject(TResult value)
        : base(true) => _value = value;

    protected internal ResultObject(ResultError error)
        : base(error) => _value = default;

    public TResult Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The operation failed. The value cannot be accesed.");

    public static implicit operator ResultObject<TResult>(TResult value) => Create(value);
}

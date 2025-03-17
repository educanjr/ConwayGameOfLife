namespace ConwayGameOfLife.Application.Exceptions;

public class ExecutionLimitReachedException : Exception
{
    public ExecutionLimitReachedException(string message) : base(message)
    {   
    }

    public ExecutionLimitReachedException() 
        : base("It is not possible to resolve the next execution. The final state has been reached, or the maximum execution limit has been exceeded.")
    {
    }
}

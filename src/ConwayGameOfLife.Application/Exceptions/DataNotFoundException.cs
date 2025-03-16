namespace ConwayGameOfLife.Application.Exceptions;

public class DataNotFoundException : Exception
{
    public DataNotFoundException(string message) : base(message)
    { }

    public DataNotFoundException(string dataNotFound, string byParam) : base($"{dataNotFound} was not found by {byParam}")
    { }
}
